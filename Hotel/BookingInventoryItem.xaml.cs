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
    /// Interaction logic for BookingInventoryItem.xaml
    /// </summary>
    public partial class BookingInventoryItem : UserControl
    {
        int inventory_type_id;
        string inventory_type_name;
        int inventory_id;
        int inventory_album_id;
        string inventory_description;
        float inventory_daily_price;

        List<DateTime> blockedList = new List<DateTime>();

        bool inventory_locked = false;

        OracleConnection con = new OracleConnection();

        BookingGuestPage bookingGuestPage;
        MainWindow mainWindow;

        public BookingInventoryItem()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();
        }

        private void InventoryItemLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT INVENTORY_TYPE_ID, INVENTORY_TYPE_NAME FROM HOTELADMIN.INVENTORY_TYPE_TABLE ORDER BY INVENTORY_TYPE_NAME";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.inventory_type_id = reader.GetInt32(0);
                this.inventory_type_name = reader.GetString(1);
                InventoryTypeComboBox.Items.Add(reader.GetString(1));
            }
            con.Close();

            if (InventoryTypeComboBox.Items.Count > 0)
            {
                InventoryTypeComboBox.SelectedItem = InventoryTypeComboBox.Items[0];
            }
        }

        private void InventoryTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.inventory_type_id = 0;
            this.inventory_type_name = null;

            if (InventoryTypeComboBox.SelectedItem == null)
            {
                return;
            }

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            bookingGuestPage = (BookingGuestPage)((Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)).MainFrame.Content;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT INVENTORY_TYPE_ID FROM HOTELADMIN.INVENTORY_TYPE_TABLE WHERE INVENTORY_TYPE_NAME = '" + InventoryTypeComboBox.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.inventory_type_id = reader.GetInt32(0);
                this.inventory_type_name = InventoryTypeComboBox.SelectedItem.ToString();
            }
            con.Close();

            InventoryComboBox.Items.Clear();
            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = 
            "SELECT INVENTORY_ID, ALBUM_ID, INVENTORY_DESCRIPTION, INVENTORY_DAILY_PRICE FROM HOTELADMIN.INVENTORY_VIEW " + 
            "WHERE INVENTORY_TYPE_ID = " + inventory_type_id + " AND " +
            "INVENTORY_ID NOT IN ( " +
            "SELECT INVENTORY_ID FROM HOTELADMIN.RENT_VIEW WHERE " +
            "RENT_START_DATE <= TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date + "', 'MM/DD/YYYY') AND " +
            "RENT_END_DATE >= TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date + "', 'MM/DD/YYYY') ) " +
            "ORDER BY INVENTORY_DESCRIPTION";
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.inventory_id = reader.GetInt32(0);
                this.inventory_album_id = reader.GetInt32(1);
                this.inventory_description = reader.GetString(2);
                this.inventory_daily_price = reader.GetFloat(3);
                InventoryComboBox.Items.Add(reader.GetString(2));
            }
            con.Close();
            if (InventoryComboBox.Items.Count > 0)
            {
                StartDateDatePicker.IsEnabled = true;
                EndDateDatePicker.IsEnabled = true;
                InventoryComboBox.SelectedItem = InventoryComboBox.Items[0];
            }
            else
            {
                StartDateDatePicker.IsEnabled = false;
                EndDateDatePicker.IsEnabled = false;
                InventoryPrice.Text = "";
                AlbumStackPanel.Visibility = Visibility.Collapsed;
                PhotoWrapPanel.Children.Clear();
            }
        }

        private void InventorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.inventory_id = 0;
            this.inventory_album_id = 0;
            this.inventory_description = null;
            this.inventory_daily_price = 0.0f;

            if (InventoryComboBox.SelectedItem == null)
            {
                return;
            }

            DateTime startDate = new DateTime(Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date.Split('/')[2]), Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date.Split('/')[0]), Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date.Split('/')[1]));
            DateTime endDate = new DateTime(Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date.Split('/')[2]), Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date.Split('/')[0]), Convert.ToInt32(((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date.Split('/')[1]));

            StartDateDatePicker.BlackoutDates.Clear();
            StartDateDatePicker.SelectedDate = startDate;
            StartDateDatePicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(1970, 1, 1), startDate.AddDays(-1)));
            StartDateDatePicker.BlackoutDates.Add(new CalendarDateRange(endDate.AddDays(1), new DateTime(9999, 12, 31)));

            EndDateDatePicker.BlackoutDates.Clear();
            EndDateDatePicker.SelectedDate = endDate;
            EndDateDatePicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(1970, 1, 1), startDate.AddDays(-1)));
            EndDateDatePicker.BlackoutDates.Add(new CalendarDateRange(endDate.AddDays(1), new DateTime(9999, 12, 31)));

            blockedList.Clear();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT INVENTORY_ID, ALBUM_ID, INVENTORY_DAILY_PRICE FROM HOTELADMIN.INVENTORY_VIEW WHERE INVENTORY_DESCRIPTION = '" + InventoryComboBox.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.inventory_id = reader.GetInt32(0);
                this.inventory_album_id = reader.GetInt32(1);
                this.inventory_description = InventoryComboBox.SelectedItem.ToString();
                this.inventory_daily_price = reader.GetFloat(2);

                InventoryPrice.Text = "Daily price: " + this.inventory_daily_price.ToString() + "$";
            }
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT PHOTO_SOURCE FROM HOTELADMIN.ALBUM_VIEW WHERE ALBUM_ID = " + inventory_album_id;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
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
                AlbumStackPanel.Visibility = Visibility.Visible;
            }
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText =
            "SELECT INVENTORY_ID, RENT_START_DATE, RENT_END_DATE FROM HOTELADMIN.RENT_VIEW " +
            "WHERE " +
            "INVENTORY_ID = " + this.inventory_id + " AND NOT " +
            "(RENT_START_DATE < TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date + "', 'MM/DD/YYYY') " +
            "AND RENT_END_DATE < TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date + "', 'MM/DD/YYYY') " +
            "OR " +
            "RENT_START_DATE > TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_end_date + "', 'MM/DD/YYYY') " +
            "AND RENT_END_DATE > TO_DATE('" + ((BookingGuestPage)mainWindow.MainFrame.Content).booking_start_date + "', 'MM/DD/YYYY') )";
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();

            List<DateTime> filterList = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                filterList.Add(date);
            }

            List<DateTime> selectList = new List<DateTime>();

            while (reader.Read())
            {
                for (var date = reader.GetDateTime(1); date <= reader.GetDateTime(2); date = date.AddDays(1))
                {
                    selectList.Add(date);
                }
            }
            con.Close();

            if (bookingGuestPage.lockedRentList.Count > 0)
            {
                foreach (Rent rent in bookingGuestPage.lockedRentList)
                {
                    if (rent.inventory_type_id == this.inventory_type_id && rent.inventory_id == this.inventory_id)
                    {
                        selectList.AddRange(rent.selectedList);
                    }
                }
            }

            selectList.Distinct();

            List<DateTime> expectList = filterList.Select(x => x.Date).Except(selectList.Select(x => x.Date)).ToList();

            if (expectList.Count > 0)
            {
                StartDateDatePicker.SelectedDate = new DateTime(expectList[0].Year, expectList[0].Month, expectList[0].Day);
                EndDateDatePicker.SelectedDate = new DateTime(expectList[0].Year, expectList[0].Month, expectList[0].Day);
            }
            else
            {
                int index = InventoryComboBox.SelectedIndex;
                int count = InventoryComboBox.Items.Count;
                if (count > index + 1)
                {
                    InventoryComboBox.SelectedItem = InventoryComboBox.Items[index + 1];
                    InventoryComboBox.Items.RemoveAt(index);
                }
                else
                {
                    InventoryComboBox.Items.RemoveAt(index);
                    StartDateDatePicker.IsEnabled = false;
                    EndDateDatePicker.IsEnabled = false;
                }
                return;
            }

            List<DateTime> intersectList = filterList.Select(x => x.Date).Intersect(selectList.Select(x => x.Date)).ToList();
            if (intersectList.Count > 0)
            {
                foreach (DateTime date in intersectList)
                {
                    StartDateDatePicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(date.Year, date.Month, date.Day), new DateTime(date.Year, date.Month, date.Day)));
                    EndDateDatePicker.BlackoutDates.Add(new CalendarDateRange(new DateTime(date.Year, date.Month, date.Day), new DateTime(date.Year, date.Month, date.Day)));
                }
                blockedList = intersectList;
            }
        }

        private void ItemChecked(object sender, RoutedEventArgs e)
        {
            bookingGuestPage.selectedRentList.Add(bookingGuestPage.RentList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent)).Parent as UIElement));
            if (bookingGuestPage.selectedRentList.Count != 0)
            {
                bookingGuestPage.RemoveRentButton.IsEnabled = true;
            }
        }

        private void ItemUnchecked(object sender, RoutedEventArgs e)
        {
            bookingGuestPage.selectedRentList.Remove(bookingGuestPage.RentList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent)).Parent as UIElement));
            if (bookingGuestPage.selectedRentList.Count == 0)
            {
                bookingGuestPage.RemoveRentButton.IsEnabled = false;
            }
        }

        private void InventoryItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StartDateDatePicker.Text == null || StartDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("This inventory cannot be locked, because you didn't enter rent start date");
                return;
            }
            if (EndDateDatePicker.Text == null || EndDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("This inventory cannot be locked, because you didn't enter rent end date");
                return;
            }

            DateTime startDate = StartDateDatePicker.SelectedDate.Value;
            DateTime endDate = EndDateDatePicker.SelectedDate.Value;

            if (this.inventory_locked)
            {
                this.inventory_locked = false;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                Border.IsEnabled = true;

                int index = -1;
                for (int i = 0; i < bookingGuestPage.lockedRentList.Count; i++)
                {
                    if (bookingGuestPage.lockedRentList[i].inventory_type_id == this.inventory_type_id &&
                        bookingGuestPage.lockedRentList[i].inventory_id == this.inventory_id &&
                        bookingGuestPage.lockedRentList[i].selectedList[0] == startDate &&
                        bookingGuestPage.lockedRentList[i].selectedList[bookingGuestPage.lockedRentList[i].selectedList.Count - 1] == endDate)
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    bookingGuestPage.lockedRentList.RemoveAt(index);
                    bookingGuestPage.RecalculateTotalPrice("Rent");
                }
            }
            else
            {
                if (this.inventory_type_name == null)
                {
                    MessageBox.Show("This inventory cannot be locked, because you didn't choose inventory type");
                    return;
                }
                if (this.inventory_description == null)
                {
                    MessageBox.Show("This inventory cannot be locked, because you didn't choose inventory");
                    return;
                }

                if (endDate < startDate)
                {
                    MessageBox.Show("This inventory cannot be locked, because rent end date less than rent start date");
                    return;
                }

                List<DateTime> selectedList = new List<DateTime>();
                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    selectedList.Add(date);
                }
                List<DateTime> intersectList = selectedList.Select(x => x.Date).Intersect(this.blockedList.Select(x => x.Date)).ToList();
                if (intersectList.Count > 0)
                {
                    MessageBox.Show("This inventory cannot be locked, because rent period has a blocked date");
                    return;
                }

                if (bookingGuestPage.lockedRentList.Count > 0)
                {
                    foreach (Rent rent in bookingGuestPage.lockedRentList)
                    {
                        if (rent.inventory_type_id == this.inventory_type_id && rent.inventory_id == this.inventory_id)
                        {
                            intersectList.Clear();
                            intersectList = selectedList.Select(x => x.Date).Intersect(rent.selectedList.Select(x => x.Date)).ToList();
                            if (intersectList.Count > 0)
                            {
                                MessageBox.Show("This inventory cannot be locked, because rent period has intersection with already locked inventory");
                                return;
                            }
                        }
                    }
                }

                this.inventory_locked = true;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
                Border.IsEnabled = false;

                bookingGuestPage.lockedRentList.Add(new Rent(this.inventory_type_id, this.inventory_type_name, this.inventory_id, this.inventory_description, this.inventory_daily_price, selectedList));
                bookingGuestPage.RecalculateTotalPrice("Rent");

                if (InventoryCheckBox.IsChecked == true)
                {
                    InventoryCheckBox.IsChecked = false;
                }
            }
        }
    }
}
