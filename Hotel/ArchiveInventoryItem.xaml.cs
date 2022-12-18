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
    /// Interaction logic for ArchiveInventoryItem.xaml
    /// </summary>
    public partial class ArchiveInventoryItem : UserControl
    {
        OracleConnection con = new OracleConnection();

        public ArchiveInventoryItem()
        {
            InitializeComponent();
        }

        public ArchiveInventoryItem(int inventory_album_id, string inventory_type_name, string inventory_description, DateTime rent_start_date, DateTime rent_end_date, float inventory_daily_price)
        {
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
            cmd.CommandText = "SELECT PHOTO_SOURCE FROM HOTELADMIN.ALBUM_VIEW WHERE ALBUM_ID = " + inventory_album_id;
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            this.Measure(new Size(898, 30));
            AlbumStackPanel.Visibility = Visibility.Collapsed;
            PhotoWrapPanel.Children.Clear();
            bool check = false;
            while (reader.Read())
            {
                try
                {
                    Image image = new Image();
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = new MemoryStream(reader.GetValue(0) as byte[]);
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;
                    image.Height = 280;
                    image.Margin = new Thickness(1, 1, 1, 1);
                    PhotoWrapPanel.Children.Add(image);
                    if (!check)
                    {
                        check = true;
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show("Photo is not found");
                }
            }
            if (check)
            {
                check = false;
                this.Measure(new Size(898, 330));
                AlbumStackPanel.Visibility = Visibility.Visible;
            }
            con.Close();

            InventoryTypeTextBox.Text = inventory_type_name;
            InventoryTextBox.Text = inventory_description;
            StartDateDatePicker.SelectedDate = rent_start_date;
            EndDateDatePicker.SelectedDate = rent_end_date;
            InventoryPrice.Text = "Daily price: " + inventory_daily_price.ToString() + "$";
        }
    }
}
