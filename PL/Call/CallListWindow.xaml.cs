using BlApi;
using BO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input; // דרוש עבור MouseButtonEventArgs

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;         // ← חשוב, אחרת Binding ל־CallList לא יעבוד
            CallList = s_bl.Call.GetCallsList();
        }

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

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (dgCalls.SelectedItem is CallInList selected)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete call {selected.Id}?",
                    "Confirm",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall((int)selected.Id);
                        MessageBox.Show(
                            "Call deleted successfully.",
                            "Success",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        CallList = s_bl.Call.GetCallsList();    // רענון הרשימה
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show(
                            $"Error deleting call: {ex.Message}",
                            "Error",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
        }

        private void btnAddCall_Click(object sender, RoutedEventArgs e)
        {
            var win = new CallWindow()
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            win.ShowDialog();
            // אחרי הוספה חוזר, רענן הרשימה
            CallList = s_bl.Call.GetCallsList();
        }

        // ה־handler ל־DoubleClick על השורה ב-DataGrid
        private void dgCalls_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgCalls.SelectedItem is CallInList selected)
            {
                var win = new CallWindow()
                {
                    Owner = this,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };
                win.ShowDialog();
                // לאחר סגירת חלון עריכה, רענן את הרשימה
                CallList = s_bl.Call.GetCallsList();
            }
        }
    }
}
