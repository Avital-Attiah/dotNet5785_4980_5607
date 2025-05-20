using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

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
    }
}
