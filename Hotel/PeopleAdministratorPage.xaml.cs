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
    /// Interaction logic for PeopleAdministratorPage.xaml
    /// </summary>
    public partial class PeopleAdministratorPage : Page
    {
        OracleConnection con = new OracleConnection();

        public Dictionary<int, string> roleDictionary = new Dictionary<int, string>();

        public PeopleAdministratorPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();
        }

        public void PeopleAdministratorPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            PeopleWrapPanel.Children.Clear();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT ROLE_ID, ROLE_NAME FROM HOTELADMIN.ROLE_TABLE WHERE ROLE_NAME <> 'Administrator' ORDER BY ROLE_ID DESC";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                roleDictionary.Add(reader.GetInt32(0), reader.GetString(1));
            }
            con.Close();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT PERSON_ID, PERSON_EMAIL, PERSON_FIRST_NAME, PERSON_LAST_NAME, PERSON_FATHER_NAME, ROLE_ID, ROLE_NAME FROM HOTELADMIN.PERSON_VIEW WHERE ROLE_NAME <> 'Administrator'";
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                PeopleWrapPanel.Children.Add(new PersonAdministratorItem(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetInt32(5), reader.GetString(6)));
            }
            con.Close();
        }
    }
}
