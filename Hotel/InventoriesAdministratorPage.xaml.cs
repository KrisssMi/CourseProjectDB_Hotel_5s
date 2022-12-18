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
    /// Interaction logic for InventoriesAdministratorPage.xaml
    /// </summary>
    public partial class InventoriesAdministratorPage : Page
    {
        OracleConnection con = new OracleConnection();

        public InventoriesAdministratorPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();
        }

        public void InventoriesAdministratorPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            SelectInventoryType();
            SelectInventory();

            con.Close();
        }

        #region InventoryTypeTable
        private void SelectInventoryType()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.INVENTORY_TYPE_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            InventoryTypeTable.ItemsSource = table.DefaultView;

            InsertInventoryTypeButton.IsEnabled = true;
            UpdateInventoryTypeButton.IsEnabled = false;
            DeleteInventoryTypeButton.IsEnabled = false;
        }

        private void InventoryTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                InventoryTypeIdTextBox.Text = rowView["INVENTORY_TYPE_ID"].ToString();
                InventoryTypeNameTextBox.Text = rowView["INVENTORY_TYPE_NAME"].ToString();
                InsertInventoryTypeButton.IsEnabled = false;
                UpdateInventoryTypeButton.IsEnabled = true;
                DeleteInventoryTypeButton.IsEnabled = true;
            }
        }

        private void InsertInventoryTypeClick(object sender, RoutedEventArgs e)
        {
            if (InventoryTypeNameTextBox.Text == String.Empty || InventoryTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory type name field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_INVENTORY_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_INVENTORY_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = InventoryTypeNameTextBox.Text.Trim();
            try
            {
                cmd.ExecuteNonQuery();
                InventoryTypeIdTextBox.Clear();
                InventoryTypeNameTextBox.Clear();
                SelectInventoryType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateInventoryTypeClick(object sender, RoutedEventArgs e)
        {
            if (InventoryTypeNameTextBox.Text == String.Empty || InventoryTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory type name field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_INVENTORY_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_INVENTORY_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_INVENTORY_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = InventoryTypeNameTextBox.Text.Trim();
            try
            {
                cmd.ExecuteNonQuery();
                InventoryTypeIdTextBox.Clear();
                InventoryTypeNameTextBox.Clear();
                SelectInventoryType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteInventoryTypeClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this inventory type?", "Delete inventory type", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_INVENTORY_TYPE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_INVENTORY_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryTypeIdTextBox.Text.Trim());
                try
                {
                    cmd.ExecuteNonQuery();
                    InventoryTypeIdTextBox.Clear();
                    InventoryTypeNameTextBox.Clear();
                    SelectInventoryType();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }
        #endregion

        #region InventoryTable
        private void SelectInventory()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.INVENTORY_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            InventoryTable.ItemsSource = table.DefaultView;

            InventoryAlbumIdTextBox.IsEnabled = false;

            InsertInventoryButton.IsEnabled = true;
            UpdateInventoryButton.IsEnabled = false;
            DeleteInventoryButton.IsEnabled = false;
        }

        private void InventorySelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                InventoryIdTextBox.Text = rowView["INVENTORY_ID"].ToString();
                InventoryInventoryTypeIdTextBox.Text = rowView["INVENTORY_INVENTORY_TYPE_ID"].ToString();
                InventoryAlbumIdTextBox.Text = rowView["INVENTORY_ALBUM_ID"].ToString();
                InventoryDescriptionTextBox.Text = rowView["INVENTORY_DESCRIPTION"].ToString();
                InventoryDailyPriceTextBox.Text = rowView["INVENTORY_DAILY_PRICE"].ToString();
                InventoryAlbumIdTextBox.IsEnabled = true;
                InsertInventoryButton.IsEnabled = false;
                UpdateInventoryButton.IsEnabled = true;
                DeleteInventoryButton.IsEnabled = true;
            }
        }

        private void InsertInventoryClick(object sender, RoutedEventArgs e)
        {
            if (InventoryInventoryTypeIdTextBox.Text == String.Empty || InventoryInventoryTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(InventoryInventoryTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill inventory type id field correctly");
                    return;
                }
            }
            if (InventoryDescriptionTextBox.Text == String.Empty || InventoryDescriptionTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory description field");
                return;
            }
            if (InventoryDailyPriceTextBox.Text == String.Empty || InventoryDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(InventoryDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill inventory daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ALBUM_NAME", OracleDbType.NVarchar2, 50).Value = InventoryDescriptionTextBox.Text;
            cmd.Parameters.Add("P_ALBUM_OBJECT", OracleDbType.Int32, 10).Value = 2;

            cmd.Parameters.Add("O_ALBUM_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_ALBUM_ID"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                int album_id;
                if (Int32.TryParse(cmd.Parameters["O_ALBUM_ID"].Value.ToString(), out album_id))
                {
                    cmd = con.CreateCommand();
                    cmd.CommandText = "HOTELADMIN.CREATE_INVENTORY";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_INVENTORY_INVENTORY_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryInventoryTypeIdTextBox.Text.Trim());
                    cmd.Parameters.Add("P_INVENTORY_ALBUM_ID", OracleDbType.Int32, 10).Value = album_id;
                    cmd.Parameters.Add("P_INVENTORY_DESCRIPTION", OracleDbType.NVarchar2, 200).Value = InventoryDescriptionTextBox.Text;
                    cmd.Parameters.Add("P_INVENTORY_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(InventoryDailyPriceTextBox.Text.Trim());
                    try
                    {
                        cmd.ExecuteNonQuery();
                        InventoryIdTextBox.Clear();
                        InventoryInventoryTypeIdTextBox.Clear();
                        InventoryAlbumIdTextBox.Clear();
                        InventoryDescriptionTextBox.Clear();
                        InventoryDailyPriceTextBox.Clear();
                        SelectInventory();
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
                    MessageBox.Show("This inventory description already exists");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateInventoryClick(object sender, RoutedEventArgs e)
        {
            if (InventoryInventoryTypeIdTextBox.Text == String.Empty || InventoryInventoryTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(InventoryInventoryTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill inventory type id field correctly");
                    return;
                }
            }
            if (InventoryAlbumIdTextBox.Text == String.Empty || InventoryAlbumIdTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory album id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(InventoryAlbumIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill inventory album id field correctly");
                    return;
                }
            }
            if (InventoryDescriptionTextBox.Text == String.Empty || InventoryDescriptionTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory description field");
                return;
            }
            if (InventoryDailyPriceTextBox.Text == String.Empty || InventoryDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill inventory daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(InventoryDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill inventory daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_INVENTORY";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_INVENTORY_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_INVENTORY_INVENTORY_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryInventoryTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_INVENTORY_ALBUM_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryAlbumIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_INVENTORY_DESCRIPTION", OracleDbType.NVarchar2, 200).Value = InventoryDescriptionTextBox.Text;
            cmd.Parameters.Add("P_INVENTORY_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(InventoryDailyPriceTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                InventoryIdTextBox.Clear();
                InventoryInventoryTypeIdTextBox.Clear();
                InventoryAlbumIdTextBox.Clear();
                InventoryDescriptionTextBox.Clear();
                InventoryDailyPriceTextBox.Clear();
                SelectInventory();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteInventoryClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this inventory?", "Delete inventory", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_INVENTORY";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_INVENTORY_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(InventoryIdTextBox.Text.Trim());
                cmd.Parameters.Add("O_INVENTORY_ALBUM_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_INVENTORY_ALBUM_ID"].Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();

                    int album_id;
                    if (Int32.TryParse(cmd.Parameters["O_INVENTORY_ALBUM_ID"].Value.ToString(), out album_id))
                    {
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
                    else
                    {
                        MessageBox.Show("Album is not found");
                    }

                    InventoryIdTextBox.Clear();
                    InventoryInventoryTypeIdTextBox.Clear();
                    InventoryAlbumIdTextBox.Clear();
                    InventoryDescriptionTextBox.Clear();
                    InventoryDailyPriceTextBox.Clear();
                    SelectInventory();
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
