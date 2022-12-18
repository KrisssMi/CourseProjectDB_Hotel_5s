using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Hotel
{
    /// <summary>
    /// Interaction logic for StaffOption.xaml
    /// </summary>
    public partial class StaffOption : UserControl
    {
        MainWindow mainWindow;

        public StaffOption()
        {
            InitializeComponent();
        }

        private void BookingClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new RoomsGuestPage();
        }

        private void ArchiveClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new ArchiveGuestPage();
        }

        private void ServicesClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new ServicesStaffPage();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new SettingsPage();
        }

        private void StaffOptionLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }
    }
}
