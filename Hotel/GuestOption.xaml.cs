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
    /// Interaction logic for GuestOption.xaml
    /// </summary>
    public partial class GuestOption : UserControl
    {
        MainWindow mainWindow;

        public GuestOption()
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

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new SettingsPage();
        }

        private void GuestOptionLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }
    }
}
