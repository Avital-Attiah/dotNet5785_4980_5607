using BlApi;
using BO;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using PL.Volunteer;
using PL.Call;
using Helpers;

namespace PL
{
    public partial class MainWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();
        private readonly DispatcherTimer _timer;
        private volatile bool _clockObserverRunning = false;
        private volatile bool _configObserverRunning = false;


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
            s_bl.Admin.StopSimulator();

        }

        // ===== משקיף על שעון המערכת =====
        private void ClockObserver()
        {
            if (!_clockObserverRunning)
            {
                _clockObserverRunning = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                    _clockObserverRunning = false;
                });
            }
        }
        // ===== משקיף על משתני תצורה =====
        private void ConfigObserver()
        {
            if (!_configObserverRunning)
            {
                _configObserverRunning = true;
                _ = Dispatcher.BeginInvoke(() =>
                {
                    RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;
                    _configObserverRunning = false;
                });
            }
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

                // ❗ עצירת הסימולטור אם רץ
                if (IsSimulatorRunning)
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }

                // סגירת חלונות
                foreach (Window w in Application.Current.Windows
                                                .OfType<Window>()
                                                .Where(w => w != this)
                                                .ToList())
                {
                    w.Close();
                }

                s_bl.Admin.InitializeDB();

                // סנכרון UI
                Dispatcher.Invoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                    RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("אירעה שגיאה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
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

                // ❗ עצירת הסימולטור אם רץ
                if (IsSimulatorRunning)
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }

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
                Dispatcher.Invoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                    RiskRangeMinutes = (int)s_bl.Admin.GetRiskRange().TotalMinutes;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("אירעה שגיאה: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void ToggleSimulator(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show($"IsSimulatorRunning = {IsSimulatorRunning}, Interval = {Interval}");
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    s_bl.Admin.StopSimulator();
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעת הפעלת/עצירת הסימולטור: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));

        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow));

    }
}
