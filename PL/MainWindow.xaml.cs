using BlApi;
using BO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PL.Volunteer;
using PL.Call;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();
        private readonly DispatcherTimer _timer;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // רישום לאירוע טעינת החלון וסגירתו
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;

            // טיימר אופציונלי שמפעיל את המשקיף כל שניה
            _timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
            _timer.Tick += (s, e) => ClockObserver();
            _timer.Start();
        }

        // ===== אירוע טעינת החלון =====
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // אתחול ערכים ראשוניים מתוך ה-BL
            CurrentTime = s_bl.Admin.GetClock();
            RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;

            // רישום משקיפים
            s_bl.Admin.AddClockObserver(ClockObserver);
            s_bl.Admin.AddConfigObserver(ConfigObserver);
        }

        // ===== אירוע סגירת החלון =====
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            // הסרת משקיפים
            s_bl.Admin.RemoveClockObserver(ClockObserver);
            s_bl.Admin.RemoveConfigObserver(ConfigObserver);
        }

        // ===== משקיף על שעון המערכת =====
        private void ClockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }

        // ===== משקיף על משתני תצורה =====
        private void ConfigObserver()
        {
            RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;
        }

        // ===== DependencyProperty עבור CurrentTime =====
        public DateTime CurrentTime
        {
            get => (DateTime)GetValue(CurrentTimeProperty);
            set => SetValue(CurrentTimeProperty, value);
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register(
                nameof(CurrentTime),
                typeof(DateTime),
                typeof(MainWindow),
                new PropertyMetadata(default(DateTime))
            );

        // ===== DependencyProperty עבור RiskRangeMinutes =====
        public int RiskRangeMinutes
        {
            get => (int)GetValue(RiskRangeMinutesProperty);
            set => SetValue(RiskRangeMinutesProperty, value);
        }
        public static readonly DependencyProperty RiskRangeMinutesProperty =
            DependencyProperty.Register(
                nameof(RiskRangeMinutes),
                typeof(int),
                typeof(MainWindow),
                new PropertyMetadata(default(int))
            );

        // ===== Handlers לכפתורי קידום שעון =====
        private void advance_minute(object sender, RoutedEventArgs e)
            => s_bl.Admin.UpdateClock(TimeUnit.MINUTE);

        private void advance_hour(object sender, RoutedEventArgs e)
            => s_bl.Admin.UpdateClock(TimeUnit.HOUR);

        private void advance_day(object sender, RoutedEventArgs e)
            => s_bl.Admin.UpdateClock(TimeUnit.DAY);

        private void advance_year(object sender, RoutedEventArgs e)
            => s_bl.Admin.UpdateClock(TimeUnit.YEAR);

        // ===== Handler לפתיחת חלון הרשימה =====
        private void View_VolunteerList(object sender, RoutedEventArgs e)
        {
            var listWindow = new VolunteerListWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            listWindow.Show();
        }

        // ===== Handler לאתחול בסיס הנתונים =====
        private void Initialize_DB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "האם אתה בטוח שברצונך לאתחל את בסיס הנתונים?",
                "אתחול בסיס נתונים",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // סגירת כל החלונות הפתוחים חוץ מהראשי
                foreach (Window w in Application.Current.Windows
                                                .OfType<Window>()
                                                .Where(w => w != this)
                                                .ToList())
                {
                    w.Close();
                }

                s_bl.Admin.InitializeDB();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        // ===== Handler לאיפוס בסיס הנתונים =====
        private void Reset_DB(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "האם אתה בטוח שברצונך לאפס את בסיס הנתונים?",
                "איפוס בסיס נתונים",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // סגירת כל החלונות הפתוחים חוץ מהראשי
                foreach (Window w in Application.Current.Windows
                                                .OfType<Window>()
                                                .Where(w => w != this)
                                                .ToList())
                {
                    w.Close();
                }

                s_bl.Admin.ResetDB();

                // סנכרון UI לאחר איפוס
                CurrentTime = s_bl.Admin.GetClock();
                RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        // ===== Handler לעדכון משתנה התצורה =====
        private void btnUpdateRiskRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(TimeSpan.FromMinutes(RiskRangeMinutes));
        }

        private void View_CallList(object sender, RoutedEventArgs e)
        {
            var listWindow = new CallListWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            listWindow.Show();
        }
    }
}
