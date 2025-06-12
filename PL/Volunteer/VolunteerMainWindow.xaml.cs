using BlApi;
using BO;
using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerMainWindow : Window
    {
        private readonly IBl _bl = Factory.Get();
        public BO.Volunteer Volunteer { get; set; }

        public VolunteerMainWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.Read(volunteerId);
            DataContext = Volunteer;
            LoadVolunteerInfo();
        }

        private void LoadVolunteerInfo()
        {
            if (Volunteer.CurrentCall != null)
            {
                txtCallDetails.Text = Volunteer.CurrentCall.ToString();
                btnSelectCall.IsEnabled = false;
                btnFinishCall.IsEnabled = true;
                btnCancelCall.IsEnabled = true;
            }
            else
            {
                txtCallDetails.Text = "אין קריאה בטיפולך";
                btnSelectCall.IsEnabled = true;
                btnFinishCall.IsEnabled = false;
                btnCancelCall.IsEnabled = false;
            }

            chkIsActive.IsEnabled = Volunteer.CurrentCall == null;
            chkIsActive.IsChecked = Volunteer.IsActive;
            txtRole.IsEnabled = false;
            txtId.IsEnabled = false;

            ShowMap();
        }

        private void ShowMap()
        {
            if (Volunteer.Latitude != 0 && Volunteer.Longitude != 0)
            {
                string mapUrl = $"https://maps.googleapis.com/maps/api/staticmap?size=600x300&markers=color:blue|{Volunteer.Latitude},{Volunteer.Longitude}";

                if (Volunteer.CurrentCall is not null && Volunteer.CurrentCall.Latitude != 0 && Volunteer.CurrentCall.Longitude != 0)
                {
                    mapUrl += $"&markers=color:red|{Volunteer.CurrentCall.Latitude},{Volunteer.CurrentCall.Longitude}";
                    mapUrl += $"&path=color:0x0000ff|weight:5|{Volunteer.Latitude},{Volunteer.Longitude}|{Volunteer.CurrentCall.Latitude},{Volunteer.CurrentCall.Longitude}";
                }

                MapBrowser.Navigate(mapUrl);
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Volunteer.IsActive = chkIsActive.IsChecked ?? true;
                _bl.Volunteer.Update(Volunteer.Id, Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelectCall_Click(object sender, RoutedEventArgs e)
        {
            new SelectCallWindow(Volunteer.Id).ShowDialog();
            Volunteer = _bl.Volunteer.Read(Volunteer.Id);
            LoadVolunteerInfo();
        }

        private void btnFinishCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.FinishCall(Volunteer.Id, Volunteer.CurrentCallId!.Value);
                MessageBox.Show("הטיפול הסתיים", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Volunteer = _bl.Volunteer.Read(Volunteer.Id);
                LoadVolunteerInfo();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CancellationOfTreatment(Volunteer.Id, Volunteer.CurrentCallId!.Value);
                MessageBox.Show("הטיפול בוטל", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Volunteer = _bl.Volunteer.Read(Volunteer.Id);
                LoadVolunteerInfo();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCallHistory_Click(object sender, RoutedEventArgs e)
        {
            new CallHistoryWindow(Volunteer.Id).ShowDialog();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
