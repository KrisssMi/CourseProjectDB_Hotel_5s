using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for ArchiveGuestItem.xaml
    /// </summary>
    public partial class ArchiveGuestItem : UserControl
    {
        int booking_id;
        DateTime booking_start_date;
        DateTime booking_end_date;
        int booking_state;
        int booking_duration;
        float booking_price;

        OracleConnection con = new OracleConnection();

        ArchiveGuestPage archiveGuestPage;
        MainWindow mainWindow;

        public ArchiveGuestItem()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
        }

        public ArchiveGuestItem(int booking_id, DateTime booking_start_date, DateTime booking_end_date, int booking_state, int booking_duration, float booking_price)
        {
            this.booking_id = booking_id;
            this.booking_start_date = booking_start_date;
            this.booking_end_date = booking_end_date;
            this.booking_state = booking_state;
            this.booking_duration = booking_duration;
            this.booking_price = booking_price;

            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ALBUM_ID, ROOM_NUMBER, ROOM_TYPE_NAME, ROOM_TYPE_DAILY_PRICE, ROOM_DESCRIPTION FROM HOTELADMIN.BOOKING_VIEW WHERE BOOKING_ID = " + booking_id;
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Number.Text = "Number: " + reader.GetString(1);
                Type.Text = "Type: " + reader.GetString(2).ToLower();
                DailyPrice.Text = "Daily price: " + reader.GetFloat(3) + "$";
                Description.Text = reader.GetString(4);

                cmd = con.CreateCommand();
                cmd.CommandText = "SELECT PHOTO_SOURCE FROM HOTELADMIN.ALBUM_VIEW WHERE ALBUM_ID = " + reader.GetInt32(0);
                cmd.CommandType = CommandType.Text;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    AlbumStackPanel.Visibility = Visibility.Visible;
                    try
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(reader.GetValue(0) as byte[]);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        image.Height = 280;
                        image.Margin = new Thickness(1, 2, 1, 2);
                        PhotoWrapPanel.Children.Add(image);
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show("Photo is not found");
                    }
                }
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

            float rent_total_price = 0.0f;
            con.Open();
            con.CreateCommand();
            cmd.CommandText = "SELECT ALBUM_ID, INVENTORY_TYPE_NAME, INVENTORY_DESCRIPTION, RENT_START_DATE, RENT_END_DATE, INVENTORY_DAILY_PRICE, RENT_PRICE FROM HOTELADMIN.RENT_VIEW WHERE BOOKING_ID = " + booking_id;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                RentList.Children.Add(new ArchiveInventoryItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetDateTime(3), reader.GetDateTime(4), reader.GetFloat(5)));
                rent_total_price += reader.GetFloat(5);
            }
            con.Close();
            if (rent_total_price == 0.0f)
            {
                Rent.Visibility = Visibility.Collapsed;
            }

            float subscription_total_price = 0.0f;
            con.Open();
            con.CreateCommand();
            cmd.CommandText = "SELECT SERVICE_TYPE_NAME, PERSON_EMAIL, SUBSCRIPTION_START_DATE, SUBSCRIPTION_END_DATE, SERVICE_TYPE_DAILY_PRICE, SUBSCRIPTION_PRICE FROM HOTELADMIN.SUBSCRIPTION_VIEW WHERE BOOKING_ID = " + booking_id;
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

            BookingInterval.Text = "Booking interval: " + booking_start_date.Month + "/" + booking_start_date.Day + "/" + booking_start_date.Year + " - " + booking_end_date.Month + "/" + booking_end_date.Day + "/" + booking_end_date.Year + " (" + booking_duration + " days)";
            TotalPrice.Text = "Total price: " + booking_price + "$";
            if (rent_total_price != 0.0f || subscription_total_price != 0.0f)
            {
                TotalPrice.Text += " (room) ";
                if (rent_total_price != 0.0f)
                {
                    TotalPrice.Text += "+ " + rent_total_price.ToString() + "$ (rent) ";
                }
                if (subscription_total_price != 0.0f)
                {
                    TotalPrice.Text += "+ " + subscription_total_price.ToString() + "$ (subscription) ";
                }
                TotalPrice.Text += "= " + (booking_price + rent_total_price + subscription_total_price).ToString() + "$";
            }

            if (booking_state == 0)
            {
                CancelBookingButton.IsEnabled = true;
            }
            else
            {
                CancelBookingButton.IsEnabled = false;
            }
        }

        private void CancelBookingClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to cancel this booking?", "Cancel booking", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.GUEST_DELETE_BOOKING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_BOOKING_ID", OracleDbType.Int32, 10).Value = booking_id;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();

                archiveGuestPage = (ArchiveGuestPage)((Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)).MainFrame.Content;
                archiveGuestPage.ArchiveGuestPageLoaded(sender, e);
            }
        }
    }
}
