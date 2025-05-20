using BlApi;
using BO;
using System.Collections.Generic;
using System.Windows;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        // 7א: שדה סטטי לגישה לשכבת ה-BL
        static readonly IBl s_bl = Factory.Get();

        public CallListWindow()
        {
            InitializeComponent();

            // 7א: קריאה למתודה ב-BL לקבלת רשימת הקריאות
            CallList = s_bl.Call.GetCallsList();
        }

        // 7ב: DependencyProperty עבור הקולקציה המקושרת ל-ItemsSource
        public IEnumerable<CallInList> CallList
        {
            get => (IEnumerable<CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register(
                nameof(CallList),
                typeof(IEnumerable<CallInList>),
                typeof(CallListWindow),
                new PropertyMetadata(null)
            );

        // Handler לכפתור Back שסוגר את החלון
        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
