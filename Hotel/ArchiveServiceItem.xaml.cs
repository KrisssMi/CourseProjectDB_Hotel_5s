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
    /// Interaction logic for ArchiveServiceItem.xaml
    /// </summary>
    public partial class ArchiveServiceItem : UserControl
    {
        public ArchiveServiceItem()
        {
            InitializeComponent();
        }

        public ArchiveServiceItem(string service_type_name, string person_email, DateTime subscription_start_date, DateTime subscription_end_date, float service_type_daily_price)
        {
            InitializeComponent();

            ServiceTypeTextBox.Text = service_type_name;
            StaffTextBox.Text = person_email;
            StartDateDatePicker.SelectedDate = subscription_start_date;
            EndDateDatePicker.SelectedDate = subscription_end_date;
            ServicePrice.Text = "Daily price: " + service_type_daily_price.ToString() + "$";
        }
    }
}
