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
    /// Interaction logic for Registration.xaml
    /// </summary>
    public partial class Registration : Window
    {
        OracleConnection con = new OracleConnection();

        public Registration()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
            InitializeComponent();
        }

        private void RegistrationClick(object sender, RoutedEventArgs e)
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
            if (PersonFirstName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill first name field");
                return;
            }
            if (PersonLastName.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill last name field");
                return;
            }
            if (PersonEmail.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill father name field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ALBUM_NAME", OracleDbType.NVarchar2, 50).Value = PersonEmail.Text.Trim();
            cmd.Parameters.Add("P_ALBUM_OBJECT", OracleDbType.Int32, 10).Value = 0;

            cmd.Parameters.Add("O_ALBUM_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_ALBUM_ID"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                int album_id;
                if (Int32.TryParse(cmd.Parameters["O_ALBUM_ID"].Value.ToString(), out album_id))
                {
                    cmd = con.CreateCommand();
                    cmd.CommandText = "HOTELADMIN.REGISTER_PERSON";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_PERSON_ALBUM_ID", OracleDbType.Int32, 10).Value = album_id;
                    cmd.Parameters.Add("P_PERSON_EMAIL", OracleDbType.NVarchar2, 50).Value = PersonEmail.Text.Trim();
                    cmd.Parameters.Add("P_PERSON_PASSWORD", OracleDbType.NVarchar2, 50).Value = PersonPassword.Password.Trim();
                    cmd.Parameters.Add("P_PERSON_FIRST_NAME", OracleDbType.NVarchar2, 50).Value = PersonFirstName.Text.Trim();
                    cmd.Parameters.Add("P_PERSON_LAST_NAME", OracleDbType.NVarchar2, 50).Value = PersonLastName.Text.Trim();
                    cmd.Parameters.Add("P_PERSON_FATHER_NAME", OracleDbType.NVarchar2, 50).Value = PersonFatherName.Text.Trim();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        Login login = new Login();
                        this.Close();
                        login.Show();
                    }
                    catch (Exception exc)
                    {
                        MessageBox.Show(exc.Message.Split('\n')[0]);
                        cmd = con.CreateCommand();
                        cmd.CommandText = "HOTELADMIN.DELETE_ALBUM";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_ALBUM_ID", OracleDbType.Int32, 10).Value = album_id;
                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception exc2)
                        {
                            MessageBox.Show(exc2.Message.Split('\n')[0]);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("This person email already exists");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void LoginClick(object sender, RoutedEventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
        }

        private void RegistrationLoaded(object sender, RoutedEventArgs e)
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
