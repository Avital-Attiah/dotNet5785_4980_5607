using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using BO;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        private ICollectionView _callsView;

        public CallListWindow()
        {
            InitializeComponent();
            LoadCalls();
        }

        private void LoadCalls()
        {
            var allCalls = BlApi.Factory.Get().Call.GetCallsList();
            _callsView = CollectionViewSource.GetDefaultView(allCalls);
            dgCalls.ItemsSource = _callsView;
        }

        private void cbStatusFilter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_callsView == null) return;
            string selected = (cbStatusFilter.SelectedItem as ComboBoxItem)?.Content.ToString();
            _callsView.Filter = c => selected == "הכל" || ((CallInList)c).Status.ToString() == selected;
        }

        private void cbSortField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_callsView == null) return;
            _callsView.SortDescriptions.Clear();
            string selected = (cbSortField.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selected)
            {
                case "סטטוס":
                    _callsView.SortDescriptions.Add(new SortDescription(nameof(CallInList.Status), ListSortDirection.Ascending));
                    break;
                case "תאריך פתיחה":
                    _callsView.SortDescriptions.Add(new SortDescription(nameof(CallInList.OpenTime), ListSortDirection.Ascending));
                    break;
                case "סוג קריאה":
                    _callsView.SortDescriptions.Add(new SortDescription(nameof(CallInList.CallType), ListSortDirection.Ascending));
                    break;
                case "מספר הקצאות":
                    _callsView.SortDescriptions.Add(new SortDescription(nameof(CallInList.AssignmentsCount), ListSortDirection.Descending));
                    break;
            }
        }

        private void cbGroupField_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_callsView == null) return;
            _callsView.GroupDescriptions.Clear();
            string selected = (cbGroupField.SelectedItem as ComboBoxItem)?.Content.ToString();
            switch (selected)
            {
                case "סטטוס":
                    _callsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CallInList.Status)));
                    break;
                case "סוג קריאה":
                    _callsView.GroupDescriptions.Add(new PropertyGroupDescription(nameof(CallInList.CallType)));
                    break;
            }
        }

        private void btnAddCall_Click(object sender, RoutedEventArgs e)
        {
            new CallWindow().ShowDialog();
            LoadCalls();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e) => Close();

        private void dgCalls_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (dgCalls.SelectedItem is CallInList selectedCall)
            {
                new CallWindow(selectedCall.CallId).ShowDialog();
                LoadCalls();
            }
        }

        private void DeleteCall_Click(object sender, RoutedEventArgs e) { /* לפי תנאים */ }
        private void CancelAssignment_Click(object sender, RoutedEventArgs e) { /* לפי תנאים */ }
    }
}
