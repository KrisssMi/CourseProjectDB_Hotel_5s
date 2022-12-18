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
    /// Interaction logic for RoomItem.xaml
    /// </summary>
    public partial class RoomItem : UserControl
    {
        int room_id;
        int room_album_id;
        string room_number;
        string room_description;
        int room_type_id;
        string room_type_name;
        int room_type_capacity;
        float room_type_daily_price;
        int duration;

        OracleConnection con = new OracleConnection();

        public RoomItem()
        {
            InitializeComponent();

            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
        }

        public RoomItem(int room_id, int room_album_id, string room_number, string room_description, int room_type_id, string room_type_name, int room_type_capacity, float room_type_daily_price, int duration)
        {
            this.room_id = room_id;
            this.room_album_id = room_album_id;
            this.room_number = room_number;
            this.room_description = room_description;
            this.room_type_id = room_type_id;
            this.room_type_name = room_type_name;
            this.room_type_capacity = room_type_capacity;
            this.room_type_daily_price = room_type_daily_price;
            this.duration = duration;

            InitializeComponent();

            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            Number.Text = "Number: " + room_number;
            Type.Text = "Type: " + room_type_name.ToLower();
            Price.Text = "Price: " + (room_type_daily_price * duration).ToString() + "$ (" + room_type_daily_price + "$ per day)";
            Description.Text = room_description;

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
        }

        private void RoomItemMouseEnter(object sender, MouseEventArgs e)
        {
            Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
        }

        private void RoomItemMouseLeave(object sender, MouseEventArgs e)
        {
            Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
        }

        private void RoomItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MainWindow mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            mainWindow.MainFrame.Content = new BookingGuestPage(room_id, room_album_id, room_number, room_description, room_type_id, room_type_name, room_type_capacity, room_type_daily_price, ((RoomsGuestPage)mainWindow.MainFrame.Content).StartDateDatePicker.Text, ((RoomsGuestPage)mainWindow.MainFrame.Content).EndDateDatePicker.Text, duration);
        }
    }
}
