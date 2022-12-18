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
    /// Interaction logic for BookingServiceItem.xaml
    /// </summary>
    public partial class BookingServiceItem : UserControl
    {
        int service_type_id;
        string service_type_name;
        float service_type_daily_price;
        int staff_id;
        string staff_email;
        int service_id;

        List<DateTime> blockedList = new List<DateTime>();

        bool service_locked = false;

        OracleConnection con = new OracleConnection();

        BookingGuestPage bookingGuestPage;
        MainWindow mainWindow;

        public BookingServiceItem()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();
        }

        private void ServiceItemLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT SERVICE_TYPE_ID, SERVICE_TYPE_NAME, SERVICE_TYPE_DAILY_PRICE FROM HOTELADMIN.SERVICE_TYPE_TABLE ORDER BY SERVICE_TYPE_NAME";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.service_type_id = reader.GetInt32(0);
                this.service_type_name = reader.GetString(1);
                this.service_type_daily_price = reader.GetFloat(2);
                ServiceTypeComboBox.Items.Add(reader.GetString(1));
            }
            con.Close();

            if (ServiceTypeComboBox.Items.Count > 0)
            {
                ServiceTypeComboBox.SelectedItem = ServiceTypeComboBox.Items[0];
            }
        }

        private void ServiceTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.service_type_id = 0;
            this.service_type_name = null;
            this.service_type_daily_price = 0.0f;

            if (ServiceTypeComboBox.SelectedItem == null)
            {
                return;
            }

            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);
            bookingGuestPage = (BookingGuestPage)((Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)).MainFrame.Content;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT SERVICE_TYPE_ID, SERVICE_TYPE_DAILY_PRICE FROM HOTELADMIN.SERVICE_TYPE_TABLE WHERE SERVICE_TYPE_NAME = '" + ServiceTypeComboBox.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.service_type_id = reader.GetInt32(0);
                this.service_type_name = ServiceTypeComboBox.SelectedItem.ToString();
                this.service_type_daily_price = reader.GetFloat(1);
                ServicePrice.Text = "Daily price: " + this.service_type_daily_price.ToString() + "$";
            }
            con.Close();

            StaffComboBox.Items.Clear();
            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = 
            "SELECT PERSON_ID, PERSON_EMAIL, SERVICE_ID FROM HOTELADMIN.SERVICE_VIEW " + 
            "WHERE SERVICE_TYPE_ID = " + service_type_id + " " +
            "ORDER BY PERSON_EMAIL";
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.staff_id = reader.GetInt32(0);
                this.staff_email = reader.GetString(1);
                this.service_id = reader.GetInt32(2);
                StaffComboBox.Items.Add(reader.GetString(1));
            }
            con.Close();
            if (StaffComboBox.Items.Count > 0)
            {
                StartDateDatePicker.IsEnabled = true;
                EndDateDatePicker.IsEnabled = true;
                StaffComboBox.SelectedItem = StaffComboBox.Items[0];
            }
            else
            {
                StartDateDatePicker.IsEnabled = false;
                EndDateDatePicker.IsEnabled = false;
                ServicePrice.Text = "";
            }
        }

        private void StaffSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.staff_id = 0;
            this.staff_email = null;
            this.service_id = 0;

            if (StaffComboBox.SelectedItem == null)
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
            cmd.CommandText = "SELECT PERSON_ID, SERVICE_ID FROM HOTELADMIN.SERVICE_VIEW WHERE PERSON_EMAIL = '" + StaffComboBox.SelectedItem.ToString() + "'";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                this.staff_id = reader.GetInt32(0);
                this.staff_email = StaffComboBox.SelectedItem.ToString();
                this.service_id = reader.GetInt32(1);
            }
            con.Close();

            List<DateTime> filterList = new List<DateTime>();
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                filterList.Add(date);
            }

            List<DateTime> selectList = new List<DateTime>();

            if (bookingGuestPage.lockedSubscriptionList.Count > 0)
            {
                foreach (Subscription subscription in bookingGuestPage.lockedSubscriptionList)
                {
                    if (subscription.service_type_id == this.service_type_id)
                    {
                        selectList.AddRange(subscription.selectedList);
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
                int index = StaffComboBox.SelectedIndex;
                int count = StaffComboBox.Items.Count;
                if (count > index + 1)
                {
                    StaffComboBox.SelectedItem = StaffComboBox.Items[index + 1];
                    StaffComboBox.Items.RemoveAt(index);
                }
                else
                {
                    StaffComboBox.Items.RemoveAt(index);
                    StartDateDatePicker.IsEnabled = false;
                    EndDateDatePicker.IsEnabled = false;
                    ServicePrice.Text = "";
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
            bookingGuestPage.selectedSubscriptionList.Add(bookingGuestPage.SubscriptionList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent as UIElement));
            if (bookingGuestPage.selectedSubscriptionList.Count != 0)
            {
                bookingGuestPage.RemoveSubscriptionButton.IsEnabled = true;
            }
        }

        private void ItemUnchecked(object sender, RoutedEventArgs e)
        {
            bookingGuestPage.selectedSubscriptionList.Remove(bookingGuestPage.SubscriptionList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent as UIElement)); 
            if (bookingGuestPage.selectedSubscriptionList.Count == 0)
            {
                bookingGuestPage.RemoveSubscriptionButton.IsEnabled = false;
            }
        }

        private void ServiceItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (StartDateDatePicker.Text == null || StartDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("This service cannot be locked, because you didn't enter subscription start date");
                return;
            }
            if (EndDateDatePicker.Text == null || EndDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("This service cannot be locked, because you didn't enter subscription end date");
                return;
            }

            DateTime startDate = StartDateDatePicker.SelectedDate.Value;
            DateTime endDate = EndDateDatePicker.SelectedDate.Value;

            if (this.service_locked)
            {
                this.service_locked = false;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                Border.IsEnabled = true;

                int index = -1;
                for (int i = 0; i < bookingGuestPage.lockedSubscriptionList.Count; i++)
                {
                    if (bookingGuestPage.lockedSubscriptionList[i].service_type_id == this.service_type_id &&
                        bookingGuestPage.lockedSubscriptionList[i].staff_id == this.staff_id &&
                        bookingGuestPage.lockedSubscriptionList[i].selectedList[0] == startDate &&
                        bookingGuestPage.lockedSubscriptionList[i].selectedList[bookingGuestPage.lockedSubscriptionList[i].selectedList.Count - 1] == endDate)
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    bookingGuestPage.lockedSubscriptionList.RemoveAt(index);
                    bookingGuestPage.RecalculateTotalPrice("Subscription");
                }
            }
            else
            {
                if (this.service_type_name == null)
                {
                    MessageBox.Show("This service cannot be locked, because you didn't choose service type");
                    return;
                }
                if (this.staff_email == null)
                {
                    MessageBox.Show("This service cannot be locked, because staff has not been found");
                    return;
                }
                foreach (Resident resident in bookingGuestPage.lockedResidentList)
                {
                    if (resident.person_email.ToUpper() == this.staff_email.ToUpper())
                    {
                        MessageBox.Show("This service cannot be locked, because staff is a resident");
                        return;
                    }
                }

                if (endDate < startDate)
                {
                    MessageBox.Show("This service cannot be locked, because subscription end date less than subscription start date");
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
                    MessageBox.Show("This service cannot be locked, because subscription period has a blocked date");
                    return;
                }

                if (bookingGuestPage.lockedSubscriptionList.Count > 0)
                {
                    foreach (Subscription subscription in bookingGuestPage.lockedSubscriptionList)
                    {
                        if (subscription.service_type_id == this.service_type_id)
                        {
                            intersectList.Clear();
                            intersectList = selectedList.Select(x => x.Date).Intersect(subscription.selectedList.Select(x => x.Date)).ToList();
                            if (intersectList.Count > 0)
                            {
                                MessageBox.Show("This service cannot be locked, because subscription period has intersection with already locked service");
                                return;
                            }
                        }
                    }
                }

                this.service_locked = true;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
                Border.IsEnabled = false;

                bookingGuestPage.lockedSubscriptionList.Add(new Subscription(this.service_type_id, this.service_type_name, this.service_type_daily_price, this.staff_id, this.staff_email, this.service_id, selectedList));
                bookingGuestPage.RecalculateTotalPrice("Subscription");

                if (ServiceCheckBox.IsChecked == true)
                {
                    ServiceCheckBox.IsChecked = false;
                }
            }
        }
    }
}
