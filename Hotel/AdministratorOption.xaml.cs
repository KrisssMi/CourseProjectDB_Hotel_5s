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
    /// Interaction logic for AdministratorOption.xaml
    /// </summary>
    public partial class AdministratorOption : UserControl
    {
        MainWindow mainWindow;

        public AdministratorOption()
        {
            InitializeComponent();
        }

        private void BookingClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new BookingAdministratorPage();
        }

        private void PeopleClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new PeopleAdministratorPage();
        }

        private void RoomsClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new RoomsAdministratorPage();
        }

        private void InventoriesClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new InventoriesAdministratorPage();
        }

        private void ServicesClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new ServicesAdministratorPage();
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            mainWindow.MainFrame.Content = new SettingsPage();
        }

        private void AdministratorOptionLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }
    }
}
