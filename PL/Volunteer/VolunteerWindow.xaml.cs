using BlApi;
using BO;
using System;
using System.Windows;
using Helpers;


namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        // 1. DependencyProperty עבור הפריט שמוצג/ערוך
        public BO.Volunteer Volunteer
        {
            get => (BO.Volunteer)GetValue(VolunteerProperty);
            set => SetValue(VolunteerProperty, value);
        }
        public static readonly DependencyProperty VolunteerProperty =
            DependencyProperty.Register(
                nameof(Volunteer),
                typeof(BO.Volunteer),
                typeof(VolunteerWindow),
                new PropertyMetadata(null)
            );

        // 2. Property להפעלת טקסט הכפתור בהתאם למצב הוספה/עדכון
        public string ButtonText { get; private set; }

        // 3. שמירה של ה־ID כדי לדעת אם מדובר בעדכון או בהוספה
        private readonly int _volunteerId;

        // פרופרטי חדש: יחזיר true אם אנחנו במצב הוספה (_volunteerId == 0), אחרת false
        public bool IsIdEnabled => _volunteerId == 0;

        // פרופרטי חדש: האם ניתן לערוך Role
        public bool IsRoleEditable => _volunteerId == 0;

        // פרופרטי חדש: האם ניתן לבטל פעיל (רק אם אין CurrentCall)
        public bool CanDeactivate => Volunteer?.CurrentCall == null;

        // 4. Constructor למצב הוספה (ID = 0)
        public VolunteerWindow()
        {
            _volunteerId = 0;
            Volunteer = new BO.Volunteer();    // אובייקט חדש
            ButtonText = "הוסף";
            InitializeComponent();
            DataContext = this;

            this.Loaded += VolunteerWindow_Loaded;
            this.Closed += VolunteerWindow_Closed;
        }

        // 5. Constructor למצב עריכה (ID != 0)
        public VolunteerWindow(int volunteerId)
        {
            _volunteerId = volunteerId;
            Volunteer = s_bl.Volunteer.Read(volunteerId)!;
            ButtonText = "עדכן";
            InitializeComponent();
            DataContext = this;

            this.Loaded += VolunteerWindow_Loaded;
            this.Closed += VolunteerWindow_Closed;
        }

        // 6. מתודת המשקיף שתתפוס עדכון מה־BL
        private void VolunteerObserver()
        {
            int id = Volunteer!.Id;
            Volunteer = null;
            Volunteer = s_bl.Volunteer.Read(id);
        }

        // 7. ברישום לאירוע Loaded – הוספה של המשקיף אם במצב עריכה
        private void VolunteerWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (_volunteerId != 0)
                s_bl.Volunteer.AddObserver(_volunteerId, VolunteerObserver);
        }

        // 8. ברישום לאירוע Closed – הסרת המשקיף
        private void VolunteerWindow_Closed(object sender, EventArgs e)
        {
            if (_volunteerId != 0)
                s_bl.Volunteer.RemoveObserver(_volunteerId, VolunteerObserver);
        }

        // 9. Handler לכפתור Create/Update
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Volunteer.Address = txtAddress.Text;

                // תרגום כתובת לקואורדינטות
                var (latitude, longitude) = CallManager.GetCoordinates(Volunteer.Address);
                Volunteer.Latitude = latitude;
                Volunteer.Longitude = longitude;

                if (_volunteerId == 0)
                    s_bl.Volunteer.Create(Volunteer);
                else
                    s_bl.Volunteer.Update(_volunteerId, Volunteer);

                MessageBox.Show(
                    $"{ButtonText} successfully",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                this.Close();
            }
            catch (BO.BlValidationException vex)
            {
                MessageBox.Show(
                    $"שגיאת ולידציה:\n{vex.Message}",
                    "שגיאה בנתונים",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לעדכן או להוסיף מתנדב בזמן שהסימולטור פועל.\n" + ex.Message,
                    "הסימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        //help to find


        // 10. Handler לכפתור ביטול/סגירה
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // 11. Handler לכפתור סיום טיפול
        private void btnCompleteCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.FinishCall(Volunteer.Id, Volunteer.CurrentCallId.Value);

                MessageBox.Show("הטיפול הסתיים בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);

                VolunteerObserver();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לסיים טיפול בזמן שהסימולטור פועל.\n" + ex.Message,
                    "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        // 12. Handler לכפתור ביטול טיפול
        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Call.FinishCall(Volunteer.Id, Volunteer.CurrentCallId.Value);

                MessageBox.Show("הטיפול בוטל", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);

                VolunteerObserver();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לבטל טיפול בזמן שהסימולטור פועל.\n" + ex.Message,
                    "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (Volunteer != null)
                Volunteer.Password = PasswordBox.Password;
        }

    }
}
