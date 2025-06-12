using BlApi;
using System.Windows;

namespace PL.Volunteer
{
    public partial class UpdateAddressWindow : Window
    {
        private readonly IBl _bl = Factory.Get();
        private readonly int _volunteerId;

        public UpdateAddressWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
        }

        private void btnUpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            var newAddress = txtNewAddress.Text.Trim();
            if (string.IsNullOrWhiteSpace(newAddress))
            {
                MessageBox.Show("יש להזין כתובת תקינה", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var volunteer = _bl.Volunteer.Read(_volunteerId);
                volunteer.Address = newAddress;
                _bl.Volunteer.Update(_volunteerId, volunteer);

                MessageBox.Show("הכתובת עודכנה בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"שגיאה בעדכון כתובת:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}