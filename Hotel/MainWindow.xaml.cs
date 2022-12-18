using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using Oracle.ManagedDataAccess.Client;

namespace Hotel
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int person_id;
        public int person_album_id;
        public string person_email;
        public string person_password;
        public string person_first_name;
        public string person_last_name;
        public string person_father_name;
        public string role_name;

        OracleConnection con = new OracleConnection();

        public MainWindow()
        {
            this.person_id = 1;
            this.person_album_id = 1;
            this.person_email = "kristina.minevich@mail.ru";
            this.person_password = "Pa$$w0rd";
            this.person_first_name = "Kristina";
            this.person_last_name = "Minevich";
            this.person_father_name = "Viktorovna";
            this.role_name = "Administrator";

            InitializeComponent();

            if (role_name == "Administrator")
            {
                con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();
                MainFrame.Content = new BookingAdministratorPage();
                OptionFrame.Content = new AdministratorOption();
            }
            else if (role_name == "Staff")
            {
                con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
                MainFrame.Content = new RoomsGuestPage();
                OptionFrame.Content = new StaffOption();
            }
            else
            {
                con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
                MainFrame.Content = new RoomsGuestPage();
                OptionFrame.Content = new GuestOption();
            }
        }

        public MainWindow(int person_id, int person_album_id, string person_email, string person_password, string person_first_name, string person_last_name, string person_father_name, string role_name)
        {
            this.person_id = person_id;
            this.person_album_id = person_album_id;
            this.person_email = person_email;
            this.person_password = person_password;
            this.person_first_name = person_first_name;
            this.person_last_name = person_last_name;
            this.person_father_name = person_father_name;
            this.role_name = role_name;

            InitializeComponent();

            if (role_name == "Administrator")
            {
                con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();
                MainFrame.Content = new BookingAdministratorPage();
                OptionFrame.Content = new AdministratorOption();
            }
            else if (role_name == "Staff")
            {
                con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
                MainFrame.Content = new RoomsGuestPage();
                OptionFrame.Content = new StaffOption();
            }
            else
            {
                con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
                MainFrame.Content = new RoomsGuestPage();
                OptionFrame.Content = new GuestOption();
            }
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
