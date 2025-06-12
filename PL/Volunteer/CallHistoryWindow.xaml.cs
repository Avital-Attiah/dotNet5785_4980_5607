using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.Volunteer
{
    public partial class CallHistoryWindow : Window
    {
        private readonly IBl _bl = Factory.Get();
        private readonly int _volunteerId;

        public CallHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadClosedCalls();
        }

        private void LoadClosedCalls()
        {
            try
            {
                List<ClosedCallInList> calls = _bl.Call
                    .GetClosedCallsByVolunteer(_volunteerId)
                    .ToList();

                lvHistory.ItemsSource = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת היסטוריית הקריאות:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
