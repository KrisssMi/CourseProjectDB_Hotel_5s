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
    /// Interaction logic for RoomsAdministratorPage.xaml
    /// </summary>
    public partial class RoomsAdministratorPage : Page
    {
        OracleConnection con = new OracleConnection();

        public RoomsAdministratorPage()
        {
            con.ConnectionString = Application.Current.FindResource("HotelAdministratorConnection").ToString();

            InitializeComponent();
        }

        public void RoomsAdministratorPageLoaded(object sender, RoutedEventArgs e)
        {
            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "ALTER SESSION SET \"_ORACLE_SCRIPT\" = TRUE";
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();

            SelectRoomType();
            SelectRoom();

            con.Close();
        }

        #region RoomTypeTable
        private void SelectRoomType()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.ROOM_TYPE_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            RoomTypeTable.ItemsSource = table.DefaultView;

            InsertRoomTypeButton.IsEnabled = true;
            UpdateRoomTypeButton.IsEnabled = false;
            DeleteRoomTypeButton.IsEnabled = false;
        }

        private void RoomTypeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                RoomTypeIdTextBox.Text = rowView["ROOM_TYPE_ID"].ToString();
                RoomTypeNameTextBox.Text = rowView["ROOM_TYPE_NAME"].ToString();
                RoomTypeCapacityTextBox.Text = rowView["ROOM_TYPE_CAPACITY"].ToString();
                RoomTypeDailyPriceTextBox.Text = rowView["ROOM_TYPE_DAILY_PRICE"].ToString();
                InsertRoomTypeButton.IsEnabled = false;
                UpdateRoomTypeButton.IsEnabled = true;
                DeleteRoomTypeButton.IsEnabled = true;
            }
        }

        private void InsertRoomTypeClick(object sender, RoutedEventArgs e)
        {
            if (RoomTypeNameTextBox.Text == String.Empty || RoomTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill room type name field");
                return;
            }
            if (RoomTypeCapacityTextBox.Text == String.Empty || RoomTypeCapacityTextBox.Text == null)
            {
                MessageBox.Show("Fill room type capacity field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(RoomTypeCapacityTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type capacity field correctly");
                    return;
                }
            }
            if (RoomTypeDailyPriceTextBox.Text == String.Empty || RoomTypeDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill room type daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(RoomTypeDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_ROOM_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ROOM_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = RoomTypeNameTextBox.Text.Trim();
            cmd.Parameters.Add("P_ROOM_TYPE_CAPACITY", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomTypeCapacityTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_TYPE_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(RoomTypeDailyPriceTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                RoomTypeIdTextBox.Clear();
                RoomTypeNameTextBox.Clear();
                RoomTypeCapacityTextBox.Clear();
                RoomTypeDailyPriceTextBox.Clear();
                SelectRoomType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateRoomTypeClick(object sender, RoutedEventArgs e)
        {
            if (RoomTypeNameTextBox.Text == String.Empty || RoomTypeNameTextBox.Text == null)
            {
                MessageBox.Show("Fill room type name field");
                return;
            }
            if (RoomTypeCapacityTextBox.Text == String.Empty || RoomTypeCapacityTextBox.Text == null)
            {
                MessageBox.Show("Fill room type capacity field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(RoomTypeCapacityTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type capacity field correctly");
                    return;
                }
            }
            if (RoomTypeDailyPriceTextBox.Text == String.Empty || RoomTypeDailyPriceTextBox.Text == null)
            {
                MessageBox.Show("Fill room type daily price field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToDouble(RoomTypeDailyPriceTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type daily price field correctly");
                    return;
                }
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_ROOM_TYPE";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ROOM_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_TYPE_NAME", OracleDbType.NVarchar2, 50).Value = RoomTypeNameTextBox.Text.Trim();
            cmd.Parameters.Add("P_ROOM_TYPE_CAPACITY", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomTypeCapacityTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_TYPE_DAILY_PRICE", OracleDbType.Double, 10).Value = Convert.ToDouble(RoomTypeDailyPriceTextBox.Text.Trim());
            try
            {
                cmd.ExecuteNonQuery();
                RoomTypeIdTextBox.Clear();
                RoomTypeNameTextBox.Clear();
                RoomTypeCapacityTextBox.Clear();
                RoomTypeDailyPriceTextBox.Clear();
                SelectRoomType();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteRoomTypeClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this room type?", "Delete room type", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_ROOM_TYPE";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ROOM_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomTypeIdTextBox.Text.Trim());
                try
                {
                    cmd.ExecuteNonQuery();
                    RoomTypeIdTextBox.Clear();
                    RoomTypeNameTextBox.Clear();
                    RoomTypeCapacityTextBox.Clear();
                    RoomTypeDailyPriceTextBox.Clear();
                    SelectRoomType();
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }
        #endregion

        #region RoomTable
        private void SelectRoom()
        {
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "SELECT * FROM HOTELADMIN.ROOM_TABLE";
            cmd.CommandType = CommandType.Text;
            OracleDataReader reader = cmd.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            RoomTable.ItemsSource = table.DefaultView;

            RoomAlbumIdTextBox.IsEnabled = false;

            InsertRoomButton.IsEnabled = true;
            UpdateRoomButton.IsEnabled = false;
            DeleteRoomButton.IsEnabled = false;
        }

        private void RoomSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid grid = sender as DataGrid;
            DataRowView rowView = grid.SelectedItem as DataRowView;
            if (rowView != null)
            {
                RoomIdTextBox.Text = rowView["ROOM_ID"].ToString();
                RoomRoomTypeIdTextBox.Text = rowView["ROOM_ROOM_TYPE_ID"].ToString();
                RoomAlbumIdTextBox.Text = rowView["ROOM_ALBUM_ID"].ToString();
                RoomNumberTextBox.Text = rowView["ROOM_NUMBER"].ToString();
                RoomDescriptionTextBox.Text = rowView["ROOM_DESCRIPTION"].ToString();
                RoomAlbumIdTextBox.IsEnabled = true;
                InsertRoomButton.IsEnabled = false;
                UpdateRoomButton.IsEnabled = true;
                DeleteRoomButton.IsEnabled = true;
            }
        }

        private void InsertRoomClick(object sender, RoutedEventArgs e)
        {
            if (RoomRoomTypeIdTextBox.Text == String.Empty || RoomRoomTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill room type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(RoomRoomTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type id field correctly");
                    return;
                }
            }
            if (RoomNumberTextBox.Text == String.Empty || RoomNumberTextBox.Text == null)
            {
                MessageBox.Show("Fill room number field");
                return;
            }
            if (RoomDescriptionTextBox.Text == String.Empty || RoomDescriptionTextBox.Text == null)
            {
                MessageBox.Show("Fill room description field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.CREATE_ALBUM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ALBUM_NAME", OracleDbType.NVarchar2, 50).Value = RoomNumberTextBox.Text.Trim();
            cmd.Parameters.Add("P_ALBUM_OBJECT", OracleDbType.Int32, 10).Value = 1;

            cmd.Parameters.Add("O_ALBUM_ID", OracleDbType.Int32, 10);
            cmd.Parameters["O_ALBUM_ID"].Direction = ParameterDirection.Output;
            try
            {
                cmd.ExecuteNonQuery();
                int album_id;
                if (Int32.TryParse(cmd.Parameters["O_ALBUM_ID"].Value.ToString(), out album_id))
                {
                    cmd = con.CreateCommand();
                    cmd.CommandText = "HOTELADMIN.CREATE_ROOM";
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add("P_ROOM_ROOM_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomRoomTypeIdTextBox.Text.Trim());
                    cmd.Parameters.Add("P_ROOM_ALBUM_ID", OracleDbType.Int32, 10).Value = album_id;
                    cmd.Parameters.Add("P_ROOM_NUMBER", OracleDbType.NVarchar2, 50).Value = RoomNumberTextBox.Text.Trim();
                    cmd.Parameters.Add("P_ROOM_DESCRIPTION", OracleDbType.NVarchar2, 200).Value = RoomDescriptionTextBox.Text;
                    try
                    {
                        cmd.ExecuteNonQuery();
                        RoomIdTextBox.Clear();
                        RoomRoomTypeIdTextBox.Clear();
                        RoomAlbumIdTextBox.Clear();
                        RoomNumberTextBox.Clear();
                        RoomDescriptionTextBox.Clear();
                        SelectRoom();
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
                    MessageBox.Show("This room number already exists");
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void UpdateRoomClick(object sender, RoutedEventArgs e)
        {
            if (RoomRoomTypeIdTextBox.Text == String.Empty || RoomRoomTypeIdTextBox.Text == null)
            {
                MessageBox.Show("Fill room type id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(RoomRoomTypeIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room type id field correctly");
                    return;
                }
            }
            if (RoomAlbumIdTextBox.Text == String.Empty || RoomAlbumIdTextBox.Text == null)
            {
                MessageBox.Show("Fill room album id field");
                return;
            }
            else
            {
                try
                {
                    Convert.ToInt32(RoomAlbumIdTextBox.Text.Trim());
                }
                catch
                {
                    MessageBox.Show("Fill room album id field correctly");
                    return;
                }
            }
            if (RoomNumberTextBox.Text == String.Empty || RoomNumberTextBox.Text == null)
            {
                MessageBox.Show("Fill room number field");
                return;
            }
            if (RoomDescriptionTextBox.Text == String.Empty || RoomDescriptionTextBox.Text == null)
            {
                MessageBox.Show("Fill room description field");
                return;
            }

            con.Open();
            OracleCommand cmd = con.CreateCommand();
            cmd.CommandText = "HOTELADMIN.UPDATE_ROOM";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add("P_ROOM_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_ROOM_TYPE_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomRoomTypeIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_ALBUM_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomAlbumIdTextBox.Text.Trim());
            cmd.Parameters.Add("P_ROOM_NUMBER", OracleDbType.NVarchar2, 50).Value = RoomNumberTextBox.Text.Trim();
            cmd.Parameters.Add("P_ROOM_DESCRIPTION", OracleDbType.NVarchar2, 200).Value = RoomDescriptionTextBox.Text;
            try
            {
                cmd.ExecuteNonQuery();
                RoomIdTextBox.Clear();
                RoomRoomTypeIdTextBox.Clear();
                RoomAlbumIdTextBox.Clear();
                RoomNumberTextBox.Clear();
                RoomDescriptionTextBox.Clear();
                SelectRoom();
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message.Split('\n')[0]);
            }
            con.Close();
        }

        private void DeleteRoomClick(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to delete this room?", "Delete room", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.DELETE_ROOM";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_ROOM_ID", OracleDbType.Int32, 10).Value = Convert.ToInt32(RoomIdTextBox.Text.Trim());
                cmd.Parameters.Add("O_ROOM_ALBUM_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_ROOM_ALBUM_ID"].Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();

                    int album_id;
                    if (Int32.TryParse(cmd.Parameters["O_ROOM_ALBUM_ID"].Value.ToString(), out album_id))
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

                    RoomIdTextBox.Clear();
                    RoomRoomTypeIdTextBox.Clear();
                    RoomAlbumIdTextBox.Clear();
                    RoomNumberTextBox.Clear();
                    RoomDescriptionTextBox.Clear();
                    SelectRoom();
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
