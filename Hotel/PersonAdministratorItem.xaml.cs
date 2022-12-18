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
    /// Interaction logic for PersonAdministratorItem.xaml
    /// </summary>
    public partial class PersonAdministratorItem : UserControl
    {
        public int person_id;
        public string person_email;
        public string person_first_name;
        public string person_last_name;
        public string person_father_name;
        public int role_id;
        public string role_name;

        OracleConnection con = new OracleConnection();

        PeopleAdministratorPage peopleAdministratorPage;

        public PersonAdministratorItem()
        {
            InitializeComponent();
        }

        public PersonAdministratorItem(int person_id, string person_email, string person_first_name, string person_last_name, string person_father_name, int role_id, string role_name)
        {
            this.person_id = person_id;
            this.person_email = person_email;
            this.person_first_name = person_first_name;
            this.person_last_name = person_last_name;
            this.person_father_name = person_father_name;
            this.role_id = role_id;
            this.role_name = role_name;

            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();

            PersonIdTextBox.Text = person_id.ToString();
            PersonEmailTextBox.Text = person_email;
            PersonFirstNameTextBox.Text = person_first_name;
            PersonLastNameTextBox.Text = person_last_name;
            PersonFatherNameTextBox.Text = person_father_name;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            peopleAdministratorPage = (PeopleAdministratorPage)((Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)).MainFrame.Content;

            int index = -1;
            for (int i = 0; i < peopleAdministratorPage.roleDictionary.Count; i++)
            {
                RoleNameComboBox.Items.Add(peopleAdministratorPage.roleDictionary.ElementAt(i).Value);
                if (peopleAdministratorPage.roleDictionary.ElementAt(i).Key == role_id)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                RoleNameComboBox.SelectedItem = RoleNameComboBox.Items[index];
            }
        }

        private void RoleNameSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = -1;
            for (int i = 0; i < peopleAdministratorPage.roleDictionary.Count; i++)
            {
                if (peopleAdministratorPage.roleDictionary.ElementAt(i).Value == RoleNameComboBox.SelectedItem.ToString())
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.UPDATE_ROLE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_PERSON_ID", OracleDbType.Int32, 10).Value = person_id;
                cmd.Parameters.Add("P_ROLE_ID", OracleDbType.Int32, 10).Value = peopleAdministratorPage.roleDictionary.ElementAt(index).Key;
                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }
    }
}
