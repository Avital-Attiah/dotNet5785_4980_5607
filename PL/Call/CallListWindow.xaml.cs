using BlApi;
using BO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Windows.Data;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        private ICollectionView _callsView;

        public CallListWindow()
        {
            InitializeComponent();
            DataContext = this;

            LoadCalls();
        }

        public IEnumerable<CallInList> CallList
        {
            get => (IEnumerable<CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }

        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register(nameof(CallList), typeof(IEnumerable<CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        private void LoadCalls()
        {
            CallList = s_bl.Call.GetCallsList();
            _callsView = CollectionViewSource.GetDefaultView(CallList);
            dgCalls.ItemsSource = _callsView;
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e)
        {
            if (dgCalls.SelectedItem is CallInList selected)
            {
                var result = MessageBox.Show($"האם אתה בטוח שברצונך למחוק את הקריאה {selected.Id}?", "אישור מחיקה", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall((int)selected.Id);
                        MessageBox.Show("הקריאה נמחקה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCalls();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"שגיאה במחיקת הקריאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CancelAssignment_Click(object sender, RoutedEventArgs e)
        {
            if (dgCalls.SelectedItem is CallInList selected)
            {
                var result = MessageBox.Show($"האם אתה בטוח שברצונך לבטל את ההקצאה לקריאה {selected.Id}?", "אישור ביטול הקצאה", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.CancelAssignment((int)selected.Id);
                        // מותר להשאיר פה שליחה של מייל אם כבר יש פונקציה — אפשר גם להסיר אם רוצים לפי דרישה
                        // s_bl.Call.SendAssignmentCancellationEmail((int)selected.Id);
                        MessageBox.Show("ההקצאה בוטלה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadCalls();
                    }
                    catch (System.Exception ex)
                    {
                        MessageBox.Show($"שגיאה בביטול ההקצאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
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
            LoadCalls();
        }

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
                LoadCalls();
            }
        }

        private void cbStatusFilter_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_callsView == null) return;

            string selectedStatus = (cbStatusFilter.SelectedItem as System.Windows.Controls.ComboBoxItem)?.Content as string;

            if (selectedStatus == "הכל")
            {
                _callsView.Filter = null;
            }
            else
            {
                _callsView.Filter = item =>
                {
                    var call = item as CallInList;
                    return call != null && call.Status.ToString() == selectedStatus;
                };
            }
        }
    }
}
