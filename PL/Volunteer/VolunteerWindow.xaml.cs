using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        // 1. DependencyProperty עבור האובייקט שמוצג/ערוך
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

        // 2. DependencyProperty עבור טקסט הכפתור ("Add" או "Update")
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register(
                nameof(ButtonText),
                typeof(string),
                typeof(VolunteerWindow),
                new PropertyMetadata("Add")
            );

        // ה־ID של המתנדב – סופר כדי להחליט אם עדכון או הוספה
        private readonly int _volunteerId;

        // בנאי למצב הוספה
        public VolunteerWindow()
        {
            _volunteerId = 0;
            ButtonText = "Add";
            Volunteer = new BO.Volunteer();  // ערכי ברירת מחדל
            InitializeComponent();
        }

        // בנאי למצב עדכון
        public VolunteerWindow(int id)
        {
            _volunteerId = id;
            ButtonText = "Update";
            Volunteer = s_bl.Volunteer.Read(id)!;  // קריאה ל-BL כדי לקבל את הפרטים הקיימים
            InitializeComponent();
        }

        // 3. Handler לכפתור Add/Update
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_volunteerId == 0)
                {
                    s_bl.Volunteer.Create(Volunteer);
                }
                else
                {
                    s_bl.Volunteer.Update(_volunteerId, Volunteer);
                }

                MessageBox.Show(
                    $"{ButtonText} succeeded",
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
