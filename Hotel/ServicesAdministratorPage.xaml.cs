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
    /// Interaction logic for ServicesAdministratorPage.xaml
    /// </summary>
    public partial class ServicesAdministratorPage : Page
    {
        OracleConnection con = new OracleConnection();

        public ServicesAdministratorPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();
        }

        public void ServicesAdministratorPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            SelectServiceType();
            SelectService();

            con.Close();
        }

        #region ServiceTypeTable
        private void SelectServiceType()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.SERVICE_TYPE_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            ServiceTypeTable.ItemsSource = table.DefaultView;

            InsertServiceTypeButton.IsEnabled = true;
            UpdateServiceTypeButton.IsEnabled = false;
            DeleteServiceTypeButton.IsEnabled = false;
        }

        private void ServiceTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                ServiceTypeIdTextBox.Text = rowView["SERVICE_TYPE_ID"].ToString();
                ServiceTypeNameTextBox.Text = rowView["SERVICE_TYPE_NAME"].ToString();
                ServiceTypeDailyPriceTextBox.Text = rowView["SERVICE_TYPE_DAILY_PRICE"].ToString();
                InsertServiceTypeButton.IsEnabled = false;
                UpdateServiceTypeButton.IsEnabled = true;
                DeleteServiceTypeButton.IsEnabled = true;
            }
        }

        private void InsertServiceTypeClick(object sender, RoutedEventArgs e)
        {
            if (ServiceTypeNameTextBox.Text == String.Empty || ServiceTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill service type name field");
                return;
            }
            if (ServiceTypeDailyPriceTextBox.Text == String.Empty || ServiceTypeDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill service type daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(ServiceTypeDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill service type daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_SERVICE_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_SERVICE_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = ServiceTypeNameTextBox.Text.Trim();
            cmd.Parameters.Add("P_SERVICE_TYPE_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(ServiceTypeDailyPriceTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                ServiceTypeIdTextBox.Clear();
                ServiceTypeNameTextBox.Clear();
                ServiceTypeDailyPriceTextBox.Clear();
                SelectServiceType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateServiceTypeClick(object sender, RoutedEventArgs e)
        {
            if (ServiceTypeNameTextBox.Text == String.Empty || ServiceTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill service type name field");
                return;
            }
            if (ServiceTypeDailyPriceTextBox.Text == String.Empty || ServiceTypeDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill service type daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(ServiceTypeDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill service type daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_SERVICE_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_SERVICE_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_SERVICE_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = ServiceTypeNameTextBox.Text.Trim();
            cmd.Parameters.Add("P_SERVICE_TYPE_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(ServiceTypeDailyPriceTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                ServiceTypeIdTextBox.Clear();
                ServiceTypeNameTextBox.Clear();
                ServiceTypeDailyPriceTextBox.Clear();
                SelectServiceType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteServiceTypeClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this service type?", "Delete service type", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_SERVICE_TYPE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_SERVICE_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceTypeIdTextBox.Text.Trim());
                try
                {
                    cmd.ExecuteNonQuery();
                    ServiceTypeIdTextBox.Clear();
                    ServiceTypeNameTextBox.Clear();
                    ServiceTypeDailyPriceTextBox.Clear();
                    SelectServiceType();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }
        #endregion

        #region ServiceTable
        private void SelectService()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.SERVICE_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            ServiceTable.ItemsSource = table.DefaultView;

            InsertServiceButton.IsEnabled = true;
            UpdateServiceButton.IsEnabled = false;
            DeleteServiceButton.IsEnabled = false;
        }

        private void ServiceSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                ServiceIdTextBox.Text = rowView["SERVICE_ID"].ToString();
                ServiceServiceTypeIdTextBox.Text = rowView["SERVICE_SERVICE_TYPE_ID"].ToString();
                ServicePersonIdTextBox.Text = rowView["SERVICE_PERSON_ID"].ToString();
                InsertServiceButton.IsEnabled = false;
                UpdateServiceButton.IsEnabled = true;
                DeleteServiceButton.IsEnabled = true;
            }
        }

        private void InsertServiceClick(object sender, RoutedEventArgs e)
        {
            if (ServiceServiceTypeIdTextBox.Text == String.Empty || ServiceServiceTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill service type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(ServiceServiceTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill service type id field correctly");
                    return;
                }
            }
            if (ServicePersonIdTextBox.Text == String.Empty || ServicePersonIdTextBox.Text == null)
            {
                MessageBox.Show("Fill person id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(ServicePersonIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill person id field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_SERVICE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_SERVICE_SERVICE_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceServiceTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_SERVICE_PERSON_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServicePersonIdTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                ServiceIdTextBox.Clear();
                ServiceServiceTypeIdTextBox.Clear();
                ServicePersonIdTextBox.Clear();
                SelectService();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateServiceClick(object sender, RoutedEventArgs e)
        {
            if (ServiceServiceTypeIdTextBox.Text == String.Empty || ServiceServiceTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill service type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(ServiceServiceTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill service type id field correctly");
                    return;
                }
            }
            if (ServicePersonIdTextBox.Text == String.Empty || ServicePersonIdTextBox.Text == null)
            {
                MessageBox.Show("Fill person id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(ServicePersonIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill person id field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_SERVICE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_SERVICE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_SERVICE_SERVICE_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceServiceTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_SERVICE_PERSON_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServicePersonIdTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                ServiceIdTextBox.Clear();
                ServiceServiceTypeIdTextBox.Clear();
                ServicePersonIdTextBox.Clear();
                SelectService();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteServiceClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this service?", "Delete service", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_SERVICE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_SERVICE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(ServiceIdTextBox.Text.Trim());
                try
                {
                    cmd.ExecuteNonQuery();
                    ServiceIdTextBox.Clear();
                    ServiceServiceTypeIdTextBox.Clear();
                    ServicePersonIdTextBox.Clear();
                    SelectService();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }
        #endregion
    }
}
