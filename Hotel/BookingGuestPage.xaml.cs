using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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

namespace Hotel
{
    /// <summary>
    /// Interaction logic for BookingGuestPage.xaml
    /// </summary>
    public partial class BookingGuestPage : Page
    {
        int room_id;
        int room_album_id;
        string room_number;
        string room_description;
        int room_type_id;
        string room_type_name;
        int room_type_capacity;
        float room_type_daily_price;
        public string booking_start_date;
        public string booking_end_date;
        int duration;

        public List<int> selectedResidentList = new List<int>();
        public List<Resident> lockedResidentList = new List<Resident>();

        public List<int> selectedRentList = new List<int>();
        public List<Rent> lockedRentList = new List<Rent>();
        public float rent_total_price;

        public List<int> selectedSubscriptionList = new List<int>();
        public List<Subscription> lockedSubscriptionList = new List<Subscription>();
        public float subscription_total_price;

        OracleConnection con = new OracleConnection();

        MainWindow mainWindow;

        public BookingGuestPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();
        }

        public BookingGuestPage(int room_id, int room_album_id, string room_number, string room_description, int room_type_id, string room_type_name, int room_type_capacity, float room_type_daily_price, string booking_start_date, string booking_end_date, int duration)
        {
            this.room_id = room_id;
            this.room_album_id = room_album_id;
            this.room_number = room_number;
            this.room_description = room_description;
            this.room_type_id = room_type_id;
            this.room_type_name = room_type_name;
            this.room_type_capacity = room_type_capacity;
            this.room_type_daily_price = room_type_daily_price;
            this.booking_start_date = booking_start_date;
            this.booking_end_date = booking_end_date;
            this.duration = duration;

            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT PHOTO_SOURCE FROM HOTELADMIN.ALBUM_VIEW WHERE ALBUM_ID = " + room_album_id;
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
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
            con.Close();

            Number.Text = "Number: " + room_number;
            Type.Text = "Type: " + room_type_name.ToLower();
            DailyPrice.Text = "Daily price: " + room_type_daily_price + "$";
            Description.Text = room_description;

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            ResidentList.Children.Add(new BookingResidentItem(mainWindow.person_id, mainWindow.person_email, mainWindow.person_first_name, mainWindow.person_last_name, mainWindow.person_father_name, true));
            if (ResidentList.Children.Count >= room_type_capacity)
            {
                AddResidentButton.IsEnabled = false;
            }

            BookingInterval.Text = "Booking interval: " + booking_start_date + " - " + booking_end_date + " ("+ duration +" days)";
            TotalPrice.Text = "Total price: " + (room_type_daily_price * duration).ToString() + "$";
        }

        private void AddResidentClick(object sender, RoutedEventArgs e)
        {
            ResidentList.Children.Add(new BookingResidentItem());
            if (ResidentList.Children.Count >= room_type_capacity)
            {
                AddResidentButton.IsEnabled = false;
            }
        }

        private void RemoveResidentClick(object sender, RoutedEventArgs e)
        {
            selectedResidentList = selectedResidentList.OrderByDescending(i => i).ToList();
            foreach (int selectedResident in selectedResidentList)
            {
                ResidentList.Children.RemoveAt(selectedResident);
            }
            selectedResidentList.Clear();
            RemoveResidentButton.IsEnabled = false;
            if (ResidentList.Children.Count < room_type_capacity)
            {
                AddResidentButton.IsEnabled = true;
            }
        }

        private void AddRentClick(object sender, RoutedEventArgs e)
        {
            RentList.Children.Add(new BookingInventoryItem());
        }

        private void RemoveRentClick(object sender, RoutedEventArgs e)
        {
            selectedRentList = selectedRentList.OrderByDescending(i => i).ToList();
            foreach (int selectedRent in selectedRentList)
            {
                RentList.Children.RemoveAt(selectedRent);
            }
            selectedRentList.Clear();
            RemoveRentButton.IsEnabled = false;
        }

        private void AddSubscriptionClick(object sender, RoutedEventArgs e)
        {
            SubscriptionList.Children.Add(new BookingServiceItem());
        }

        private void RemoveSubscriptionClick(object sender, RoutedEventArgs e)
        {
            selectedSubscriptionList = selectedSubscriptionList.OrderByDescending(i => i).ToList();
            foreach (int selectedSubscription in selectedSubscriptionList)
            {
                SubscriptionList.Children.RemoveAt(selectedSubscription);
            }
            selectedSubscriptionList.Clear();
            RemoveSubscriptionButton.IsEnabled = false;
        }

        public void RecalculateTotalPrice(string mode)
        {
            if (mode == "Rent")
            {
                rent_total_price = 0.0f;
                foreach (Rent rent in lockedRentList)
                {
                    rent_total_price += rent.inventory_daily_price * rent.selectedList.Count();
                }
            }
            else if (mode == "Subscription")
            {
                subscription_total_price = 0.0f;
                foreach (Subscription subscription in lockedSubscriptionList)
                {
                    subscription_total_price += subscription.service_type_daily_price * subscription.selectedList.Count();
                }
            }
            TotalPrice.Text = "Total price: " + (room_type_daily_price * duration).ToString() + "$";
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
                TotalPrice.Text += "= " + (room_type_daily_price * duration + rent_total_price + subscription_total_price).ToString() + "$";
            }
            
        }

        private void BookClick(object sender, RoutedEventArgs e)
        {
            bool booking_error = false;

            if (MessageBox.Show("Do you really want to book this room?", "Room booking", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {

                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.GUEST_CREATE_BOOKING";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_BOOKING_ROOM_ID", OracleDbType.Int32, 10).Value = room_id;
                cmd.Parameters.Add("P_BOOKING_START_DATE", OracleDbType.Date, 7).Value = new DateTime(Convert.ToInt32(booking_start_date.Split('/')[2]), Convert.ToInt32(booking_start_date.Split('/')[0]), Convert.ToInt32(booking_start_date.Split('/')[1]));
                cmd.Parameters.Add("P_BOOKING_END_DATE", OracleDbType.Date, 7).Value = new DateTime(Convert.ToInt32(booking_end_date.Split('/')[2]), Convert.ToInt32(booking_end_date.Split('/')[0]), Convert.ToInt32(booking_end_date.Split('/')[1]));

                cmd.Parameters.Add("O_BOOKING_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_BOOKING_ID"].Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();
                    int booking_id = Convert.ToInt32(cmd.Parameters["O_BOOKING_ID"].Value.ToString());
                    if (booking_id > 0)
                    {
                        //Resident
                        if (lockedResidentList.Count > 0)
                        {
                            foreach (Resident lockedResident in lockedResidentList)
                            {
                                if (!booking_error)
                                {
                                    cmd = con.CreateCommand();
                                    cmd.CommandText = "HOTELADMIN.CREATE_RESIDENT";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("P_RESIDENT_PERSON_ID", OracleDbType.Int32, 10).Value = lockedResident.person_id;
                                    cmd.Parameters.Add("P_RESIDENT_BOOKING_ID", OracleDbType.Int32, 10).Value = booking_id;
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception exc)
                                    {
                                        MessageBox.Show(exc.Message.Split('\n')[0]);
                                        booking_error = true;
                                    }
                                }
                            }
                        }
                        //Rent
                        if (lockedRentList.Count > 0)
                        {
                            foreach (Rent lockedRent in lockedRentList)
                            {
                                if (!booking_error)
                                {
                                    cmd = con.CreateCommand();
                                    cmd.CommandText = "HOTELADMIN.CREATE_RENT";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("P_RENT_INVENTORY_ID", OracleDbType.Int32, 10).Value = lockedRent.inventory_id;
                                    cmd.Parameters.Add("P_RENT_BOOKING_ID", OracleDbType.Int32, 10).Value = booking_id;
                                    cmd.Parameters.Add("P_RENT_START_DATE", OracleDbType.Date, 7).Value = lockedRent.selectedList[0];
                                    cmd.Parameters.Add("P_RENT_END_DATE", OracleDbType.Date, 7).Value = lockedRent.selectedList[lockedRent.selectedList.Count - 1];
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception exc)
                                    {
                                        MessageBox.Show(exc.Message.Split('\n')[0]);
                                        booking_error = true;
                                    }
                                }
                            }
                        }
                        //Subscription
                        if (lockedSubscriptionList.Count > 0)
                        {
                            foreach (Subscription lockedSubscription in lockedSubscriptionList)
                            {
                                if (!booking_error)
                                {
                                    cmd = con.CreateCommand();
                                    cmd.CommandText = "HOTELADMIN.CREATE_SUBSCRIPTION";
                                    cmd.CommandType = CommandType.StoredProcedure;
                                    cmd.Parameters.Add("P_SUBSCRIPTION_SERVICE_ID", OracleDbType.Int32, 10).Value = lockedSubscription.service_id;
                                    cmd.Parameters.Add("P_SUBSCRIPTION_BOOKING_ID", OracleDbType.Int32, 10).Value = booking_id;
                                    cmd.Parameters.Add("P_SUBSCRIPTION_START_DATE", OracleDbType.Date, 7).Value = lockedSubscription.selectedList[0];
                                    cmd.Parameters.Add("P_SUBSCRIPTION_END_DATE", OracleDbType.Date, 7).Value = lockedSubscription.selectedList[lockedSubscription.selectedList.Count - 1];
                                    try
                                    {
                                        cmd.ExecuteNonQuery();
                                    }
                                    catch (Exception exc)
                                    {
                                        MessageBox.Show(exc.Message.Split('\n')[0]);
                                        booking_error = true;
                                    }
                                }
                            }
                        }
                        if (booking_error)
                        {
                            cmd = con.CreateCommand();
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
                        }
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();

                if (!booking_error)
                {
                    mainWindow.MainFrame.Content = new ArchiveGuestPage();
                }
            }
        }
    }
}
