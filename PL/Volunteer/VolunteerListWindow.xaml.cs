using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        // תכונה לשמירת הפריט הנבחר ברשימה
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        // תכונה לסינון לפי סוג קריאה
        public CallType SelectedCallType { get; set; } = CallType.None;

        // DependencyProperty עבור הקולקציה שתוצג ב־ListView
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
            DataContext = this;    // ← שורה חדשה כדי שה־XAML יקשר נכון ל־this

            QueryVolunteerList();
            this.Loaded += VolunteerListWindow_Loaded;
            this.Closed += VolunteerListWindow_Closed;
        }

        // קריאה חוזרת לטעינת הרשימה
        private void VolunteerListObserver()
        {
            QueryVolunteerList();
        }

        // מטפל בסינון – אם SelectedCallType != None, מסנן אחרת מחזיר הכל
        private void QueryVolunteerList()
        {
            var all = s_bl.Volunteer.GetVolunteersList();
            if (SelectedCallType == CallType.None)
                VolunteerList = all;
            else
                VolunteerList = all.Where(v => v.CurrentCallType == SelectedCallType).ToList();
        }

        // ברגע שהחלון נטען – מוסיפים משקיף
        private void VolunteerListWindow_Loaded(object? sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        // בעת סגירת החלון – מסירים משקיף
        private void VolunteerListWindow_Closed(object? sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        // SelectionChanged של ComboBox – מעדכן את הרשימה
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            QueryVolunteerList();
        }

        // Handler לכפתור Back (סגירת החלון)
        private void btnBack_Click(object sender, RoutedEventArgs e) => this.Close();

        // Handler לכפתור Add (פותח חלון הוספה של מתנדב חדש)
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new VolunteerWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
        }

        // כשעושים DoubleClick על שורת רשימה – פותחים עריכה של המתנדב הנבחר
        private void lsvCoursesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var win = new VolunteerWindow(SelectedVolunteer.Id)
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                win.ShowDialog();
            }
        }

        // Handler לכפתור Delete בתוך ה־ListView
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedVolunteer == null)
                return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete the volunteer {SelectedVolunteer.FullName}?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.Delete(SelectedVolunteer.Id);
                    MessageBox.Show(
                        "The volunteer was successfully deleted.",
                        "Success",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    // לאחר מחיקה, רענון הרשימה
                    QueryVolunteerList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"Error deleting volunteer: {ex.Message}",
                        "Error",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
    }
}
