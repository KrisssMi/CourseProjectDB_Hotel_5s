using Oracle.ManagedDataAccess.Client;
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

namespace Hotel
{
    /// <summary>
    /// Interaction logic for RoomsGuestPage.xaml
    /// </summary>
    public partial class RoomsGuestPage : Page
    {
        OracleConnection con = new OracleConnection();

        public RoomsGuestPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();
        }

        private void RoomsGuestPageLoaded(object sender, RoutedEventArgs e)
        {
            StartDateDatePicker.SelectedDate = DateTime.Now;
            StartDateDatePicker.BlackoutDates.AddDatesInPast();

            EndDateDatePicker.SelectedDate = DateTime.Now;
            EndDateDatePicker.BlackoutDates.AddDatesInPast();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ROOM_TYPE_NAME FROM HOTELADMIN.ROOM_TYPE_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                RoomTypeComboBox.Items.Add(reader.GetString(0));
            }
            con.Close();
            RoomTypeComboBox.SelectedItem = RoomTypeComboBox.Items[0];
        }

        private void NumericTextBoxValidation(object sender, TextCompositionEventArgs e)
        {
            e.Handled = new Regex("[^0-9]+").IsMatch(e.Text);
        }

        private void FindClick(object sender, RoutedEventArgs e)
        {
            if (DailyPriceFromTextBox.Text == String.Empty)
            {
                MessageBox.Show("Fill minimum price for one day field");
                return;
            }
            if (StartDateDatePicker.Text == null || StartDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("Fill start date field");
                return;
            }
            if (EndDateDatePicker.Text == null || EndDateDatePicker.Text == String.Empty)
            {
                MessageBox.Show("Fill end date field");
                return;
            }


            DateTime startDate = Convert.ToDateTime(StartDateDatePicker.Text);
            DateTime endDate = Convert.ToDateTime(EndDateDatePicker.Text);

            if (endDate < startDate)
            {
                MessageBox.Show("End date less than start date");
                return;
            }

            RoomsWrapPanel.Children.Clear();
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText =
            "SELECT ROOM_ID, ALBUM_ID, ROOM_NUMBER, ROOM_DESCRIPTION, ROOM_TYPE_ID, ROOM_TYPE_NAME, ROOM_TYPE_CAPACITY, ROOM_TYPE_DAILY_PRICE FROM HOTELADMIN.ROOM_VIEW " +
            "WHERE " +
            "ROOM_TYPE_NAME = '" + RoomTypeComboBox.SelectedItem.ToString() + "' AND " +
            "ROOM_TYPE_DAILY_PRICE >= " + DailyPriceFromTextBox.Text + " AND " +
            "ROOM_TYPE_DAILY_PRICE <= " + DailyPriceToTextBox.Text + " AND " +
            "ROOM_ID NOT IN( " +
            "SELECT ROOM_ID FROM HOTELADMIN.BOOKING_VIEW WHERE NOT " +
            "(BOOKING_START_DATE<TO_DATE('" + EndDateDatePicker.Text + "', 'MM/DD/YYYY') " +
            "AND BOOKING_END_DATE<TO_DATE('" + StartDateDatePicker.Text + "', 'MM/DD/YYYY') " +
            "OR " +
            "BOOKING_START_DATE > TO_DATE('" + EndDateDatePicker.Text + "', 'MM/DD/YYYY') " +
            "AND BOOKING_END_DATE > TO_DATE('" + StartDateDatePicker.Text + "', 'MM/DD/YYYY') ) " +
            ")" +
            "ORDER BY ROOM_ID";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                RoomsWrapPanel.Children.Add(new RoomItem(reader.GetInt32(0), reader.GetInt32(1), reader.GetString(2), reader.GetString(3), reader.GetInt32(4), reader.GetString(5), reader.GetInt32(6), reader.GetFloat(7), endDate.Subtract(startDate).Days + 1));
            }
            con.Close();
        }
    }
}
