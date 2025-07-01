using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PL.Volunteer
{
    public partial class SelectCallWindow : Window
    {
        private readonly IBl _bl = Factory.Get();
        private readonly int _volunteerId;

        public SelectCallWindow(int volunteerId)
        {
            InitializeComponent();
            _volunteerId = volunteerId;
            LoadOpenCalls();
        }

        private void LoadOpenCalls()
        {
            try
            {
                IEnumerable<OpenCallInList> calls = _bl.Call
                    .GetOpenCalls(_volunteerId)
                    .ToList();

                lvCalls.ItemsSource = calls;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת קריאות פתוחות:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            if (lvCalls.SelectedItem is OpenCallInList selectedCall)
            {
                try
                {
                    _bl.Call.SelectCall(_volunteerId, selectedCall.Id);
                    MessageBox.Show("הקריאה נבחרה בהצלחה", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
                }
                catch (BO.BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show("לא ניתן לבחור קריאה בזמן שהסימולטור פועל.\n" + ex.Message,
                        "הסימולטור פעיל", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בבחירת הקריאה:\n{ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
            else
            {
                MessageBox.Show("אנא בחר קריאה מהרשימה", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
