using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
    /// Interaction logic for BookingResidentItem.xaml
    /// </summary>
    public partial class BookingResidentItem : UserControl
    {
        int resident_id;
        int resident_album_id;
        string resident_email;
        string resident_first_name;
        string resident_last_name;
        string resident_father_name;

        bool resident_locked = false;

        OracleConnection con = new OracleConnection();

        BookingGuestPage bookingGuestPage;

        public BookingResidentItem()
        {
            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();
        }

        public BookingResidentItem(int resident_id, string resident_email, string resident_first_name, string resident_last_name, string resident_father_name, bool isMain)
        {
            this.resident_id = resident_id;
            this.resident_email = resident_email;
            this.resident_first_name = resident_first_name;
            this.resident_last_name = resident_last_name;
            this.resident_father_name = resident_father_name;

            con.ConnectionString = Application.Current.FindResource("HotelGuestConnection").ToString();

            InitializeComponent();

            ResidentEmailTextBox.Text = resident_email;
            ResidentFirstNameTextBox.Text = resident_first_name;
            ResidentLastNameTextBox.Text = resident_last_name;
            ResidentFatherNameTextBox.Text = resident_father_name;

            if (isMain)
            {
                this.resident_locked = true;
                MouseDoubleClick -= ResidentItemMouseDoubleClick;
            }
        }

        private void ResidentItemLoaded(object sender, RoutedEventArgs e)
        {
            bookingGuestPage = (BookingGuestPage)((Hotel.MainWindow)Application.Current.Windows.OfType<Window>().SingleOrDefault(x => x.IsActive)).MainFrame.Content;

            if (this.resident_locked)
            {
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
                Border.IsEnabled = false;
                bookingGuestPage.lockedResidentList.Add(new Resident(this.resident_id, this.resident_email, this.resident_first_name, this.resident_last_name, this.resident_father_name));
            }
        }

        private void ResidentChecked(object sender, RoutedEventArgs e)
        {
            bookingGuestPage.selectedResidentList.Add(bookingGuestPage.ResidentList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent as UIElement));
            if (bookingGuestPage.selectedResidentList.Count != 0)
            {
                bookingGuestPage.RemoveResidentButton.IsEnabled = true;
            }
        }

        private void ResidentUnchecked(object sender, RoutedEventArgs e)
        {
            bookingGuestPage.selectedResidentList.Remove(bookingGuestPage.ResidentList.Children.IndexOf(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)(((FrameworkElement)((UIElement)sender)).Parent)).Parent)).Parent)).Parent as UIElement)); 
            if (bookingGuestPage.selectedResidentList.Count == 0)
            {
                bookingGuestPage.RemoveResidentButton.IsEnabled = false;
            }
        }

        private void ResidentEmailEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                this.resident_id = 0;
                this.resident_email = null;
                this.resident_first_name = null;
                this.resident_last_name = null;
                this.resident_father_name = null;

                ResidentFirstNameTextBox.Clear();
                ResidentLastNameTextBox.Clear();
                ResidentFatherNameTextBox.Clear();

                foreach (Resident resident in bookingGuestPage.lockedResidentList)
                {
                    if (resident.person_email.ToUpper() == ResidentEmailTextBox.Text.Trim().ToUpper())
                    {
                        MessageBox.Show("This resident has already fixed");
                        return;
                    }
                }

                con.Open();
                OracleCommand cmd = con.CreateCommand();
                cmd.CommandText = "HOTELADMIN.SEARCH_PERSON";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("P_PERSON_EMAIL", OracleDbType.NVarchar2, 50).Value = ResidentEmailTextBox.Text.Trim();

                cmd.Parameters.Add("O_PERSON_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_PERSON_ID"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("O_PERSON_ALBUM_ID", OracleDbType.Int32, 10);
                cmd.Parameters["O_PERSON_ALBUM_ID"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("O_PERSON_EMAIL", OracleDbType.NVarchar2, 50);
                cmd.Parameters["O_PERSON_EMAIL"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("O_PERSON_FIRST_NAME", OracleDbType.NVarchar2, 50);
                cmd.Parameters["O_PERSON_FIRST_NAME"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("O_PERSON_LAST_NAME", OracleDbType.NVarchar2, 50);
                cmd.Parameters["O_PERSON_LAST_NAME"].Direction = ParameterDirection.Output;
                cmd.Parameters.Add("O_PERSON_FATHER_NAME", OracleDbType.NVarchar2, 50);
                cmd.Parameters["O_PERSON_FATHER_NAME"].Direction = ParameterDirection.Output;
                try
                {
                    cmd.ExecuteNonQuery();
                    this.resident_id = Convert.ToInt32(cmd.Parameters["O_PERSON_ID"].Value.ToString());
                    this.resident_album_id = Convert.ToInt32(cmd.Parameters["O_PERSON_ALBUM_ID"].Value.ToString());
                    this.resident_email = Convert.ToString(cmd.Parameters["O_PERSON_EMAIL"].Value);
                    this.resident_first_name = Convert.ToString(cmd.Parameters["O_PERSON_FIRST_NAME"].Value);
                    this.resident_last_name = Convert.ToString(cmd.Parameters["O_PERSON_LAST_NAME"].Value);
                    this.resident_father_name = Convert.ToString(cmd.Parameters["O_PERSON_FATHER_NAME"].Value);

                    ResidentEmailTextBox.Text = resident_email;
                    ResidentFirstNameTextBox.Text = resident_first_name;
                    ResidentLastNameTextBox.Text = resident_last_name;
                    ResidentFatherNameTextBox.Text = resident_father_name;
                }
                catch (Exception exc)
                {
                    MessageBox.Show(exc.Message.Split('\n')[0]);
                }
                con.Close();
            }
        }

        private void ResidentItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (this.resident_locked)
            {
                this.resident_locked = false;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F5F5F5"));
                Border.IsEnabled = true;

                int index = -1;
                for (int i = 0; i < bookingGuestPage.lockedResidentList.Count; i++)
                {
                    if (bookingGuestPage.lockedResidentList[i].person_email == this.resident_email)
                    {
                        index = i;
                    }
                }
                if (index != -1)
                {
                    bookingGuestPage.lockedResidentList.RemoveAt(index);
                }
            }
            else
            {
                if (this.resident_email == null)
                {
                    MessageBox.Show("This resident cannot be locked, because person has not been found");
                    return;
                }
                foreach (Resident resident in bookingGuestPage.lockedResidentList)
                {
                    if (resident.person_email == this.resident_email)
                    {
                        MessageBox.Show("This resident has already locked");
                        return;
                    }
                }
                foreach (Subscription subscription in bookingGuestPage.lockedSubscriptionList)
                {
                    if (subscription.staff_email.ToUpper() == this.resident_email.ToUpper())
                    {
                        MessageBox.Show("This resident cannot be locked, because resident is a staff");
                        return;
                    }
                }

                this.resident_locked = true;
                Border.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFC8C8C8"));
                Border.IsEnabled = false;
                bookingGuestPage.lockedResidentList.Add(new Resident(this.resident_id, this.resident_email, this.resident_first_name, this.resident_last_name, this.resident_father_name));

                if (ResidentCheckBox.IsChecked == true)
                {
                    ResidentCheckBox.IsChecked = false;
                }
            }
        }
    }
}
