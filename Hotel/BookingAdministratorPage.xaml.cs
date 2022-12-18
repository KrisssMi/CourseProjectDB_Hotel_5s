﻿using Oracle.ManagedDataAccess.Client;
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
    /// Interaction logic for BookingAdministratorPage.xaml
    /// </summary>
    public partial class BookingAdministratorPage : Page
    {
        OracleConnection con = new OracleConnection();

        public BookingAdministratorPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();
        }

        public void BookingAdministratorPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            BookingWrapPanel.Children.Clear();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT BOOKING_ID, BOOKING_START_DATE, BOOKING_END_DATE, BOOKING_STATE, BOOKING_DURATION, BOOKING_PRICE FROM HOTELADMIN.BOOKING_VIEW WHERE BOOKING_END_DATE >= TO_DATE('" + DateTime.Now.Day + "/ "+ DateTime.Now.Month +"/" + DateTime.Now.Year + "', 'DD/MM/YYYY')";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                BookingWrapPanel.Children.Add(new BookingAdministratorItem(reader.GetInt32(0), reader.GetDateTime(1), reader.GetDateTime(2), reader.GetInt32(3), reader.GetInt32(4), reader.GetFloat(5)));
            }
            con.Close();
        }
    }
}
