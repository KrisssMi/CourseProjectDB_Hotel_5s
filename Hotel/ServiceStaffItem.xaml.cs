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
    /// Interaction logic for ServiceStaffItem.xaml
    /// </summary>
    public partial class ServiceStaffItem : UserControl
    {
        int booking_id;
        int subscription_id;
        DateTime subscription_start_date;
        DateTime subscription_end_date;
        int subscription_duration;
        float subscription_price;

        OracleConnection con = new OracleConnection();

        ServicesStaffPage servicesStaffPage;
        MainWindow mainWindow;

        public ServiceStaffItem()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }

        public ServiceStaffItem(int booking_id, int subscription_id, DateTime subscription_start_date, DateTime subscription_end_date, int subscription_duration, float subscription_price)
        {
            this.booking_id = booking_id;
            this.subscription_id = subscription_id;
            this.subscription_start_date = subscription_start_date;
            this.subscription_end_date = subscription_end_date;
            this.subscription_duration = subscription_duration;
            this.subscription_price = subscription_price;

            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ROOM_NUMBER, ROOM_TYPE_NAME, ROOM_TYPE_DAILY_PRICE, ROOM_DESCRIPTION FROM HOTELADMIN.BOOKING_VIEW WHERE BOOKING_ID = " + booking_id;
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Number.Text = "Number: " + reader.GetString(0);
                Type.Text = "Type: " + reader.GetString(1).ToLower();
                Description.Text = reader.GetString(3);
            }
            con.Close();

            con.Open();
            con.CreateCommand();
            cmd.CommandText = "SELECT PERSON_EMAIL, PERSON_FIRST_NAME, PERSON_LAST_NAME, PERSON_FATHER_NAME FROM HOTELADMIN.RESIDENT_VIEW WHERE BOOKING_ID = " + booking_id;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                ResidentList.Children.Add(new ArchiveResidentItem(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            con.Close();

            float subscription_total_price = 0.0f;
            con.Open();
            con.CreateCommand();
            cmd.CommandText = "SELECT SERVICE_TYPE_NAME, PERSON_EMAIL, SUBSCRIPTION_START_DATE, SUBSCRIPTION_END_DATE, SERVICE_TYPE_DAILY_PRICE, SUBSCRIPTION_PRICE FROM HOTELADMIN.SUBSCRIPTION_VIEW WHERE SUBSCRIPTION_ID = " + subscription_id;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                SubscriptionList.Children.Add(new ArchiveServiceItem(reader.GetString(0), reader.GetString(1), reader.GetDateTime(2), reader.GetDateTime(3), reader.GetFloat(4)));
                subscription_total_price += reader.GetFloat(5);
            }
            con.Close();
            if (subscription_total_price == 0.0f)
            {
                Subscription.Visibility = Visibility.Collapsed;
            }

            TotalPrice.Text = "Subscription price: " + subscription_price + "$";
        }
    }
}
