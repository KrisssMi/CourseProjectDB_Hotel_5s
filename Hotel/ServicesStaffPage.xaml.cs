using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Interaction logic for ServicesStaffPage.xaml
    /// </summary>
    public partial class ServicesStaffPage : Page
    {
        OracleConnection con = new OracleConnection();

        MainWindow mainWindow;

        public ServicesStaffPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }

        public void ServicesStaffPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            ServicesWrapPanel.Children.Clear();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT BOOKING_ID, SUBSCRIPTION_ID, SUBSCRIPTION_START_DATE, SUBSCRIPTION_END_DATE, SUBSCRIPTION_DURATION, SUBSCRIPTION_PRICE FROM HOTELADMIN.SUBSCRIPTION_VIEW WHERE PERSON_ID = " + mainWindow.person_id + " AND SUBSCRIPTION_END_DATE >= TO_DATE('" + DateTime.Now.Day + "/ " + DateTime.Now.Month + "/" + DateTime.Now.Year + "', 'DD/MM/YYYY') ORDER BY SUBSCRIPTION_START_DATE ASC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ServicesWrapPanel.Children.Add(new ServiceStaffItem(reader.GetInt32(0), reader.GetInt32(1), reader.GetDateTime(2), reader.GetDateTime(3), reader.GetInt32(4), reader.GetFloat(5)));
            }
            con.Close();
        }
    }
}
