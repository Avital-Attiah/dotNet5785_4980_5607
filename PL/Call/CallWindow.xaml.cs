using BlApi;
using BO;
using System;
using System.Windows;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        // DependencyProperty עבור אובייקט ה־Call שמוצג/נערך
        public BO.Call Call
        {
            get => (BO.Call)GetValue(CallProperty);
            set => SetValue(CallProperty, value);
        }
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register(
                nameof(Call),
                typeof(BO.Call),
                typeof(CallWindow),
                new PropertyMetadata(null)
            );

        private readonly int _callId;

        // Constructor מצב הוספה
        public CallWindow()
        {
            _callId = 0;
            Call = new BO.Call();
            Call.Status = CallStatus.Open;
            InitializeComponent();
            DataContext = this; // ← שורה חיונית ל־Bindings ב־XAML
        }

        // Constructor מצב עריכה (ID != 0)
        public CallWindow(int callId)
        {
            _callId = callId;
            Call = s_bl.Call.Read(callId)!; // קריאה ראשונית ל־BL
            InitializeComponent();
            DataContext = this; // ← שורה חיונית
        }

        // Handler לכפתור Create/Update
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_callId == 0)
                    s_bl.Call.Create(Call);
                else
                    s_bl.Call.UpdateCallDetails(Call);

                MessageBox.Show(
                    $"{(_callId == 0 ? "הוספה" : "עדכון")} בוצע בהצלחה",
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

        // Handler לכפתור ביטול/סגירה
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
