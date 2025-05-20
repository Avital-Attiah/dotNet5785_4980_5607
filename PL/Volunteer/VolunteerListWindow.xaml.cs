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
        public BO.VolunteerInList? SelectedVolunteer { get; set; }


        // 1. תכונה רגילה לאחסון ערך הסינון (SelectedCallType) ומתאם ל-None
        public CallType SelectedCallType { get; set; } = CallType.None;

        // 2. DependencyProperty לאגירת הרשימה שמוצגת
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

            // אתחול ראשוני של הרשימה
            QueryVolunteerList();

            // רישום לאירוע טעינת החלון
            this.Loaded += VolunteerListWindow_Loaded;
            // רישום לאירוע סגירת החלון
            this.Closed += VolunteerListWindow_Closed;
        }

        // 3. מתודת השקפה על הרשימה - קוראת מחדש את הרשימה המסוננת
        private void VolunteerListObserver()
        {
            QueryVolunteerList();
        }

        // 4. שאילתת טעינת הרשימה (עם סינון)
        private void QueryVolunteerList()
        {
            var all = s_bl.Volunteer.GetVolunteersList();
            if (SelectedCallType == CallType.None)
                VolunteerList = all;
            else
                VolunteerList = all.Where(v => v.CurrentCallType == SelectedCallType).ToList();
        }

        // 5. אירוע Loaded - הוספת משקיף
        private void VolunteerListWindow_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
        }

        // 6. אירוע Closed - הסרת משקיף
        private void VolunteerListWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(VolunteerListObserver);
        }

        // 7. Handler לאירוע SelectionChanged של ה־ComboBox
        private void ComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // הערך SelectedCallType עודכן אוטומטית דרך Binding
            QueryVolunteerList();
        }

        // 8. Handler לכפתור חזרה
        private void btnBack_Click(object sender, RoutedEventArgs e)
            => this.Close();

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var win = new VolunteerWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            win.Show();
        }


        private void lsvCoursesList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var win = new VolunteerWindow(SelectedVolunteer.Id)
                {
                    Owner = this    // אופציונלי, כדי ש-VolunteerListWindow יישאר הבעלים
                };
                win.ShowDialog();  // <-- כאן במקום Show()
            }

        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {

            if (SelectedVolunteer == null)
                return;

            var result = MessageBox.Show($"Are you sure you want to delete the volunteer? {SelectedVolunteer.FullName}?",
                                         "Confirm",
                                         MessageBoxButton.YesNo,
                                         MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteer.Delete(SelectedVolunteer.Id);
                    MessageBox.Show("The volunteer was successfully deleted.", "success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting volunteer:" + ex.Message, "error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

