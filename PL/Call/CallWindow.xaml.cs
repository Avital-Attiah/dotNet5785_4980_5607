using BlApi;
using BO;
using Helpers;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PL.Call
{
    public partial class CallWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public BO.Call Call
        {
            get => (BO.Call)GetValue(CallProperty);
            set => SetValue(CallProperty, value);
        }

        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register(nameof(Call), typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public string ButtonLabel => _callId == 0 ? "➕ הוספה" : "✏️ עדכון";

        private readonly int _callId;

        public CallWindow()
        {
            _callId = 0;
            Call = new BO.Call
            {
                Status = CallStatus.Open,
                OpenTime = AdminManager.Now,

            };
            InitializeComponent();
            DataContext = this;
        }

        public CallWindow(int callId)
        {
            _callId = callId;
            Call = s_bl.Call.Read(callId);
            InitializeComponent();
            DataContext = this;
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // חישוב MaxCompletionTime לפי מספר הדקות שהוזנו
                if (int.TryParse(txtMaxMinutes.Text, out int minutes))
                {
                    Call.MaxCompletionTime = Call.OpenTime.AddMinutes(minutes);
                }
                else
                {
                    Call.MaxCompletionTime = null; // אם השדה ריק או לא מספר
                }

                // יצירה או עדכון
                if (_callId == 0)
                    s_bl.Call.Create(Call);
                else
                    s_bl.Call.UpdateCallDetails(Call);

                MessageBox.Show("הפעולה בוצעה בהצלחה", "✔", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן להוסיף או לעדכן קריאה בזמן שהסימולטור פועל.\n" + ex.Message,
                    "הסימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה: {ex.Message}", "⚠", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) => Close();
    }

    public class IdToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int id && id > 0) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
