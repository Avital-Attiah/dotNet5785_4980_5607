using BlApi;
using BO;
using System.Windows;

namespace PL.Volunteer
{
    public partial class LoginWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            // 1. קבלת ת.ז.
            string idText = txtUserId.Text.Trim();
            if (string.IsNullOrEmpty(idText))
            {
                MessageBox.Show("אנא הזן ת.ז.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(idText, out int userId))
            {
                MessageBox.Show("ת.ז. לא תקינה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. נסיון קריאה של Volunteer (אין Manager נפרד)
            BO.Volunteer volunteer;
            try
            {
                volunteer = s_bl.Volunteer.Read(userId);
            }
            catch
            {
                volunteer = null;
            }

            if (volunteer != null)
            {
                // 3. במקום IsManager(), נשווה את volunteer.Role לערך Role.Manager
                if (volunteer.Role == Role.Manager)
                {
                    // בחרת מנהל – תן למנהל לבחור מסך ניהול או מסך מתנדב
                    var result = MessageBox.Show(
                        "שלום מנהל!\n\nהאם תרצה להיכנס למסך ניהול ראשי?",
                        "בחירת מסך",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // כניסה למסך הניהול הראשי (MainWindow)
                        var adminWin = new MainWindow();
                        adminWin.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        // כניסה למסך המתנדב (VolunteerWindow) כמתנדב מנהל
                        var volWin = new VolunteerWindow(userId)
                        {
                            Owner = this,
                            WindowStartupLocation = WindowStartupLocation.CenterOwner
                        };
                        volWin.ShowDialog();
                        return;
                    }
                }
                else
                {
                    // 4. מתנדב רגיל – נכנס ישר למסך המתנדב (VolunteerWindow או Dashboard)
                    var volWin = new VolunteerWindow(userId)
                    {
                        Owner = this,
                        WindowStartupLocation = WindowStartupLocation.CenterOwner
                    };
                    volWin.ShowDialog();
                    return;
                }
            }

            // 5. אם לא נמצא Volunteer, אין צורך בבדיקת Manager נפרד
            MessageBox.Show("משתמש לא נמצא במערכת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
