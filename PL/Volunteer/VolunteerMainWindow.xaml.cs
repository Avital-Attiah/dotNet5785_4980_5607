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
            if (Volunteer.CurrentCallId.HasValue)
            {
                try
                {
                    var call = _bl.Call.Read(Volunteer.CurrentCallId.Value);

                    Volunteer.CurrentCall = new CallInProgress
                    {
                        Id = call.Id,
                        CallType = call.CallType,
                        Description = call.Description,
                        FullAddress = call.FullAddress,
                        // הוסיפי כאן שדות נוספים לפי הצורך
                    };
                }
                catch
                {
                    Volunteer.CurrentCall = null!;
                }
            }

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
        }



        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Volunteer.IsActive = chkIsActive.IsChecked ?? true;
                _bl.Volunteer.Update(Volunteer.Id, Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לעדכן פרטי מתנדב בזמן שהסימולטור פועל.\n" + ex.Message,
                    "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
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
                int? callId = Volunteer.CurrentCallId ?? Volunteer.CurrentCall?.Id;
                if (callId == null)
                    throw new Exception("לא קיימת קריאה בטיפול.");

                _bl.Call.FinishCall(Volunteer.Id, callId.Value);
                MessageBox.Show("הטיפול הסתיים", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Volunteer = _bl.Volunteer.Read(Volunteer.Id);
                LoadVolunteerInfo();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לסיים טיפול בזמן הסימולטור.\n" + ex.Message,
                    "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int? callId = Volunteer.CurrentCallId ?? Volunteer.CurrentCall?.Id;
                if (callId == null)
                    throw new Exception("לא קיימת קריאה לטיפול.");

                _bl.Call.CancellationOfTreatment(Volunteer.Id, callId.Value);
                MessageBox.Show("הטיפול בוטל", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Volunteer = _bl.Volunteer.Read(Volunteer.Id);
                LoadVolunteerInfo();
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show("לא ניתן לבטל טיפול בזמן הסימולטור.\n" + ex.Message,
                    "סימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
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
