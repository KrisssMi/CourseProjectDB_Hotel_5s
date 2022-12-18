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
using Oracle.ManagedDataAccess.Client;

namespace Hotel
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        OracleConnection con = new OracleConnection();

        public Login()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
            InitializeComponent();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            if (PersonEmail.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill email field");
                return;
            }
            else
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(PersonEmail.Text.Trim());
                if (!match.Success)
                {
                    MessageBox.Show("Email is invalid");
                    return;
                }
            }
            if (PersonPassword.Password.Trim() == String.Empty)
            {
                MessageBox.Show("Fill password field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.LOG_IN_PERSON";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_PERSON_EMAIL", OracleDbType.NVarchar2, 50).Value = PersonEmail.Text.Trim();
            cmd.Parameters.Add("P_PERSON_PASSWORD", OracleDbType.NVarchar2, 50).Value = PersonPassword.Password.Trim();

            cmd.Parameters.Add("O_PERSON_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_PERSON_ID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_ALBUM_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_PERSON_ALBUM_ID"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_EMAIL", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_PERSON_EMAIL"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_PASSWORD", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_PERSON_PASSWORD"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_FIRST_NAME", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_PERSON_FIRST_NAME"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_LAST_NAME", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_PERSON_LAST_NAME"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_PERSON_FATHER_NAME", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_PERSON_FATHER_NAME"].Direction = ParameterDirection.Output;
            cmd.Parameters.Add("O_ROLE_NAME", OracleDbType.NVarchar2, 50);
            cmd.Parameters["O_ROLE_NAME"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                int person_id = Convert.ToInt32(cmd.Parameters["O_PERSON_ID"].Value.ToString());
                int person_album_id = Convert.ToInt32(cmd.Parameters["O_PERSON_ALBUM_ID"].Value.ToString());
                string person_email = Convert.ToString(cmd.Parameters["O_PERSON_EMAIL"].Value);
                string person_password = Convert.ToString(cmd.Parameters["O_PERSON_PASSWORD"].Value);
                string person_first_name = Convert.ToString(cmd.Parameters["O_PERSON_FIRST_NAME"].Value);
                string person_last_name = Convert.ToString(cmd.Parameters["O_PERSON_LAST_NAME"].Value);
                string person_father_name = Convert.ToString(cmd.Parameters["O_PERSON_FATHER_NAME"].Value);
                string role_name = Convert.ToString(cmd.Parameters["O_ROLE_NAME"].Value);

                MainWindow mainWindow = new MainWindow(person_id, person_album_id, person_email, person_password, person_first_name, person_last_name, person_father_name, role_name);
                mainWindow.Show();
                this.Close();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void RegistrationClick(object sender, RoutedEventArgs e)
        {
            Registration registration = new Registration();
            registration.Show();
            this.Close();
        }

        private void LoginLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
