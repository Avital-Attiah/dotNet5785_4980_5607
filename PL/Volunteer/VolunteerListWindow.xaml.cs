using BlApi;
using BO;                     // כדי לגשת ל־enum VolunteerInLIstFields ול־CallType
using PL;                     // כדי לגשת ל־VolunteerInLIstFieldsCollection ול־CallTypesCollection
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        private readonly IBl s_bl = Factory.Get();

        // הפריט הנבחר מתוך הרשימה (לעדכון/מחיקה/עריכה)
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        // סינון לפי סוג קריאה (CallType)
        public CallType SelectedCallType { get; set; } = CallType.None;

        // מיון לפי שדה מתוך ה־enum VolunteerInLIstFields (Nullable כדי לאפשר "ללא מיון")
        public VolunteerInLIstFields? SelectedSortField { get; set; } = null;

        // הקולקציה שמוצגת ב־ListView (כתלות DependencyProperty כדי לתמוך ב־Binding)
        public IEnumerable<VolunteerInList> VolunteerList
        {
            get => (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }
        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register(
                nameof(VolunteerList),
                typeof(IEnumerable<VolunteerInList>),
                typeof(VolunteerListWindow),
                new PropertyMetadata(null)
            );


        public VolunteerListWindow()
        {
            InitializeComponent();
            DataContext = this;    // הגדרת ה־DataContext אל ה־Window עצמו

            QueryVolunteerList();  // טען בפעם הראשונה
            this.Loaded += VolunteerListWindow_Loaded;
            this.Closed += VolunteerListWindow_Closed;
        }

        // מתבצע כאשר החלון נטען על המסך – מוסיפים משקיף (Observer) עבור שינויים ברשימת המתנדבים
        private void VolunteerListWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        // מתבצע כאשר החלון נסגר – מסירים את המשקיף
        private void VolunteerListWindow_Closed(object? sender, System.EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        // המשקיף נקרא כשחלה כל עדכון ברשימת המתנדבים ב־BL
        private volatile bool _volunteerListObserverRunning = false; // שלב 7

        private void VolunteerListObserver()
        {
            if (!_volunteerListObserverRunning)
            {
                _volunteerListObserverRunning = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    QueryVolunteerList();
                    _volunteerListObserverRunning = false;
                });
            }
        }


        // Handler לשינוי הבחירה ב־ComboBox של סינון לפי CallType
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryVolunteerList();
        }

        // Handler לשינוי הבחירה ב־ComboBox של מיון לפי שדה מתוך VolunteerInLIstFields
        private void SortFields_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryVolunteerList();
        }

        // קורא ויוצר את הרשימה המעודכנת, עם סינון + מיון
        private void QueryVolunteerList()
        {
            // 1. קבלת כל המתנדבים (ללא סינון)
            var all = s_bl.Volunteer.GetVolunteersList();

            // 2. סינון לפי CallType (אם נבחר ערך שונה מ־None)
            IEnumerable<VolunteerInList> filtered =
                (SelectedCallType == CallType.None)
                ? all
                : all.Where(v => v.CurrentCallType == SelectedCallType);

            // 3. מיון לפי SelectedSortField (אם לא null)
            if (SelectedSortField.HasValue)
            {
                switch (SelectedSortField.Value)
                {
                    case VolunteerInLIstFields.Id:
                        filtered = filtered.OrderBy(v => v.Id);
                        break;

                    case VolunteerInLIstFields.FullName:
                        filtered = filtered.OrderBy(v => v.FullName);
                        break;

                    case VolunteerInLIstFields.IsActive:
                        // פעילים (true) יופיעו לפני לא פעילים (false)
                        filtered = filtered.OrderByDescending(v => v.IsActive);
                        break;

                    case VolunteerInLIstFields.TotalHandledCalls:
                        filtered = filtered.OrderByDescending(v => v.TotalHandledCalls);
                        break;

                    case VolunteerInLIstFields.TotalCancelledCalls:
                        filtered = filtered.OrderByDescending(v => v.TotalCanceledCalls);
                        break;

                    case VolunteerInLIstFields.TotalExpiredSelectedCalls:
                        filtered = filtered.OrderByDescending(v => v.TotalExpiredCalls);
                        break;

                    case VolunteerInLIstFields.CallId:
                        filtered = filtered.OrderBy(v => v.CurrentCallId);
                        break;

                    case VolunteerInLIstFields.TypeCall:
                        // סדר לפי השם של CurrentCallType (כדי להציג לפי סדר אלפביתי)
                        filtered = filtered.OrderBy(v => v.CurrentCallType.ToString());
                        break;

                    default:
                        break;
                }
            }
            // אם SelectedSortField == null => נשאר ללא מיון

            // 4. הצבת התוצאה ב־DependencyProperty כדי שה־ListView יתעדכן אוטומטית
            VolunteerList = filtered.ToList();
        }

        // Handler לכפתור “הוסף” – פותח את חלון הוספת מתנדב חדש (VolunteerWindow ללא ID)
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new VolunteerWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.Show();
        }

        // Handler לכפתור “חזור” – סוגר את החלון הנוכחי
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // מתבצע כשעושים DoubleClick על שורה ב־ListView – פותח חלון עריכה של אותו מתנדב
        private void lsvCoursesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var win = new VolunteerWindow(SelectedVolunteer.Id)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                win.Show();
            }
        }

        // Handler לכפתור Delete בתוך שורת ה־ListView – מבצע מחיקה
        //private void DeleteButton_Click(object sender, RoutedEventArgs e)
        //{
        //    if (SelectedVolunteer == null)
        //        return;

        //    var result = MessageBox.Show(
        //        $"האם את בטוחה שאת רוצה למחוק את המתנדב “{SelectedVolunteer.FullName}”?",
        //        "אישור מחיקה",
        //        MessageBoxButton.YesNo,
        //        MessageBoxImage.Warning);

        //    if (result == MessageBoxResult.Yes)
        //    {
        //        try
        //        {
        //            s_bl.Volunteer.Delete(SelectedVolunteer.Id);
        //            MessageBox.Show(
        //                "המתנדב נמחק בהצלחה.",
        //                "הצלחה",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Information);

        //            // לאחר מחיקה – רענון הרשימה
        //            QueryVolunteerList();
        //        }
        //        catch (System.Exception ex)
        //        {
        //            MessageBox.Show(
        //                $"שגיאה במחיקת המתנדב: {ex.Message}",
        //                "שגיאה",
        //                MessageBoxButton.OK,
        //                MessageBoxImage.Error);
        //        }
        //    }
        //}

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer == null)
                return;

            // 1. קבלת כל הקריאות
            var allCalls = s_bl.Call.GetCallsList(); // מניח שמוחזר IEnumerable<CallInList>

            // 2. בדיקה אם קיים Call פעיל עבור המתנדב הזה
            bool hasActiveCall = false;
            foreach (var c in allCalls)
            {
                // מניח ש-CallInList כולל VolunteerId ושדה Status מסוג CallStatus
                if (c.Id == SelectedVolunteer.Id &&
                    (c.Status == CallStatus.Open || c.Status == CallStatus.InProgress))
                {
                    hasActiveCall = true;
                    break;
                }
            }

            if (hasActiveCall)
            {
                MessageBox.Show(
                    "לא ניתן למחוק את המתנדב – הוא מטפל כרגע בקריאה פעילה.",
                    "מחיקה לא אפשרית",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // 3. אם אין קריאה פעילה – אפשר להמשיך למחיקה
            var result = MessageBox.Show(
                $"האם את בטוחה שאת רוצה למחוק את המתנדב “{SelectedVolunteer.FullName}”?",
                "אישור מחיקה",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.Delete(SelectedVolunteer.Id);
                    MessageBox.Show(
                        "המתנדב נמחק בהצלחה.",
                        "הצלחה",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);

                    QueryVolunteerList(); // ריענון הרשימה לאחר המחיקה
                }
                catch (BO.BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show("לא ניתן למחוק מתנדב בזמן שהסימולטור פועל.\n" + ex.Message,
                        "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה במחיקת המתנדב: {ex.Message}",
                        "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }


    }
}
