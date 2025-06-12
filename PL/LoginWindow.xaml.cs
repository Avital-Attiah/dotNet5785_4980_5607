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
                // 3. בדיקת תפקיד
                if (volunteer.Role == Role.Manager)
                {
                    var result = MessageBox.Show(
                        "שלום מנהל!\n\nהאם תרצה להיכנס למסך ניהול ראשי?",
                        "בחירת מסך",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var adminWin = new MainWindow();
                        adminWin.Show();
                        this.Close();
                        return;
                    }
                    else
                    {
                        // מנהל בוחר להיכנס כמתנדב → שולח ל־VolunteerMainWindow
                        var volMainWin = new VolunteerMainWindow(userId);
                        volMainWin.Show();
                        this.Close();
                        return;
                    }
                }
                else
                {
                    // מתנדב רגיל → ישר ל־VolunteerMainWindow
                    var volMainWin = new VolunteerMainWindow(userId);
                    volMainWin.Show();
                    this.Close();
                    return;
                }
            }

            // 4. לא נמצא מתנדב
            MessageBox.Show("משתמש לא נמצא במערכת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
        }

    }
}
