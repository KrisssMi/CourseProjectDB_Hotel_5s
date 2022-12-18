using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
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
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : Page
    {
        bool isChange = false;

        OracleConnection con = new OracleConnection();

        MainWindow mainWindow;

        public SettingsPage()
        {
            InitializeComponent();
        }

        public void ChangePersonClick(object sender, RoutedEventArgs e)
        {
            if (isChange)
            {
                isChange = false;

                PersonEmailTextBox.Text = mainWindow.person_email;
                PersonPasswordTextBox.Text = mainWindow.person_password;
                PersonFirstNameTextBox.Text = mainWindow.person_first_name;
                PersonLastNameTextBox.Text = mainWindow.person_last_name;
                PersonFatherNameTextBox.Text = mainWindow.person_father_name;

                PersonEmailTextBox.IsEnabled = false;
                PersonPasswordTextBox.IsEnabled = false;
                PersonFirstNameTextBox.IsEnabled = false;
                PersonLastNameTextBox.IsEnabled = false;
                PersonFatherNameTextBox.IsEnabled = false;

                ChangePersonButton.Content = "Change person info";
                UpdatePersonButton.IsEnabled = false;
            }
            else
            {
                isChange = true;

                PersonEmailTextBox.IsEnabled = true;
                PersonPasswordTextBox.IsEnabled = true;
                PersonFirstNameTextBox.IsEnabled = true;
                PersonLastNameTextBox.IsEnabled = true;
                PersonFatherNameTextBox.IsEnabled = true;

                ChangePersonButton.Content = "Revert person changes";
                UpdatePersonButton.IsEnabled = true;
            }
        }

        public void UpdatePersonClick(object sender, RoutedEventArgs e)
        {
            if (PersonEmailTextBox.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill email field");
                return;
            }
            else
            {
                Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                Match match = regex.Match(PersonEmailTextBox.Text.Trim());
                if (!match.Success)
                {
                    MessageBox.Show("Email is invalid");
                    return;
                }
            }
            if (PersonPasswordTextBox.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill password field");
                return;
            }
            if (PersonFirstNameTextBox.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill first name field");
                return;
            }
            if (PersonLastNameTextBox.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill last name field");
                return;
            }
            if (PersonEmailTextBox.Text.Trim() == String.Empty)
            {
                MessageBox.Show("Fill father name field");
                return;
            }

            if (MessageBox.Show("Do you really want to update person info?", "Update person info", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.UPDATE_PERSON";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_PERSON_ID", OracleDbType.Int32, 10).Value = mainWindow.person_id;
                cmd.Parameters.Add("P_PERSON_ALBUM_ID", OracleDbType.Int32, 10).Value = mainWindow.person_album_id;
                cmd.Parameters.Add("P_PERSON_EMAIL", OracleDbType.NVarchar2, 50).Value = PersonEmailTextBox.Text.Trim();
                cmd.Parameters.Add("P_PERSON_PASSWORD", OracleDbType.NVarchar2, 50).Value = PersonPasswordTextBox.Text.Trim();
                cmd.Parameters.Add("P_PERSON_FIRST_NAME", OracleDbType.NVarchar2, 50).Value = PersonFirstNameTextBox.Text.Trim();
                cmd.Parameters.Add("P_PERSON_LAST_NAME", OracleDbType.NVarchar2, 50).Value = PersonLastNameTextBox.Text.Trim();
                cmd.Parameters.Add("P_PERSON_FATHER_NAME", OracleDbType.NVarchar2, 50).Value = PersonFatherNameTextBox.Text.Trim();
                try
                {
                    cmd.ExecuteNonQuery();

                    isChange = false;

                    mainWindow.person_email = PersonEmailTextBox.Text;
                    mainWindow.person_password = PersonPasswordTextBox.Text;
                    mainWindow.person_first_name = PersonFirstNameTextBox.Text;
                    mainWindow.person_last_name = PersonLastNameTextBox.Text;
                    mainWindow.person_father_name = PersonFatherNameTextBox.Text;

                    PersonEmailTextBox.IsEnabled = false;
                    PersonPasswordTextBox.IsEnabled = false;
                    PersonFirstNameTextBox.IsEnabled = false;
                    PersonLastNameTextBox.IsEnabled = false;
                    PersonFatherNameTextBox.IsEnabled = false;

                    UpdatePersonButton.IsEnabled = false;
                    ChangePersonButton.Content = "Change person info";
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }

        public void DeletePersonClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this person?", "Delete person", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_PERSON";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_PERSON_ID", OracleDbType.Int32, 10).Value = mainWindow.person_id;
                cmd.Parameters.Add("O_PERSON_ALBUM_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_PERSON_ALBUM_ID"].Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();

                    int album_id;
                    if (Int32.TryParse(cmd.Parameters["O_PERSON_ALBUM_ID"].Value.ToString(), out album_id))
                    {
                        cmd = con.CreateCommand();
                        cmd.CommandText = "HOTELADMIN.DELETE_ALBUM";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add("P_ALBUM_ID", OracleDbType.Int32, 10).Value = album_id;
                        try
                        {
                            cmd.ExecuteNonQuery();

                            Login login = new Login();
                            login.Show();
                            mainWindow.Close();
                        }
                        catch (Exception exc2)
                        {
                            MessageBox.Show(exc2.Message.Split('\n')[0]);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Album is not found");
                    }
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }

        public void LogoutClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to log out?", "Log out", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                Login login = new Login();
                login.Show();
                mainWindow.Close();
            }
        }

        public void ExportInventoryTypeClick(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.EXPORT_INVENTORY_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Export was successfully completed");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        public void ImportInventoryTypeClick(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.IMPORT_INVENTORY_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
                MessageBox.Show("Import was successfully completed");
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        public void Insert100kRoleClick(object sender, RoutedEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.INSERT_100K";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            MessageBox.Show("Execution time: " + elapsedTime);
        }

        public void Delete100kRoleClick(object sender, RoutedEventArgs e)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.DELETE_100K";
            cmd.CommandType = CommandType.StoredProcedure;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();

            stopWatch.Stop();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            MessageBox.Show("Execution time: " + elapsedTime);
        }

        private void SettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            mainWindow = (Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive);

            if (mainWindow.role_name == "Administrator")
            {
                con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();
                DeletePersonButton.IsEnabled = false;
            }
            else
            {
                con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();
                InventoryType.Visibility = Visibility.Collapsed;
                MultipleRole.Visibility = Visibility.Collapsed;
            }

            PersonEmailTextBox.Text = mainWindow.person_email;
            PersonPasswordTextBox.Text = mainWindow.person_password;
            PersonFirstNameTextBox.Text = mainWindow.person_first_name;
            PersonLastNameTextBox.Text = mainWindow.person_last_name;
            PersonFatherNameTextBox.Text = mainWindow.person_father_name;

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            con.Close();

            List<string> services = new List<string>();

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT SERVICE_TYPE_NAME FROM HOTELADMIN.SERVICE_VIEW WHERE PERSON_ID = " + mainWindow.person_id;
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                services.Add(reader.GetString(0).ToLower());
            }
            con.Close();

            if (services.Count > 0)
            {
                ServiceNameTextBox.Visibility = Visibility.Visible;
                foreach (string service in services)
                {
                    ServiceNameTextBox.Text += " " + service + ",";
                }
                ServiceNameTextBox.Text = ServiceNameTextBox.Text.Substring(0, ServiceNameTextBox.Text.Length - 1);
            }

            con.Open();
            cmd = con.CreateCommand();
            cmd.CommandText = "SELECT PHOTO_SOURCE FROM HOTELADMIN.ALBUM_VIEW WHERE ALBUM_ID = " + mainWindow.person_album_id;
            cmd.CommandType = CommandType.Text;
            reader = cmd.ExecuteReader();
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
    }
}
