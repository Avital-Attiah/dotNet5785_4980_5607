using BlApi;
using BO;
using System;
using System.Windows;

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

        // 4. Constructor למצב הוספה (ID = 0)
        public VolunteerWindow()
        {
            _volunteerId = 0;
            Volunteer = new BO.Volunteer();    // אובייקט חדש
            ButtonText = "הוסף";
            InitializeComponent();

            this.Loaded += VolunteerWindow_Loaded;
            this.Closed += VolunteerWindow_Closed;
        }

        // 5. Constructor למצב עריכה (ID != 0)
        public VolunteerWindow(int volunteerId)
        {
            _volunteerId = volunteerId;
            Volunteer = s_bl.Volunteer.Read(volunteerId)!;   // קריאה ראשונית
            ButtonText = "עדכן";
            InitializeComponent();

            this.Loaded += VolunteerWindow_Loaded;
            this.Closed += VolunteerWindow_Closed;
        }

        // 6. מתודת המשקיף שתתפוס עדכון מה־BL
        private void VolunteerObserver()
        {
            int id = Volunteer!.Id;
            Volunteer = null;                              // כדי לגרום ל־Binding להתעדכן
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
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Error: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}
