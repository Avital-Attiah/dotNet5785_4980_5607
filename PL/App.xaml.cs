using System;
using System.Windows;

namespace PL
{
    public partial class App : Application
    {
        public App()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"שגיאה שלא נתפסה:\n{e.Exception.GetType().Name}: {e.Exception.Message}",
                "שגיאה כללית", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                MessageBox.Show($"שגיאה בלתי צפויה:\n{ex.GetType().Name}: {ex.Message}",
                    "שגיאת מערכת", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
