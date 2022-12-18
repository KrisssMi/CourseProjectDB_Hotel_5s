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
    /// Interaction logic for ArchiveResidentItem.xaml
    /// </summary>
    public partial class ArchiveResidentItem : UserControl
    {
        public ArchiveResidentItem()
        {
            InitializeComponent();
        }

        public ArchiveResidentItem(string person_email, string person_first_name, string person_last_name, string person_father_name)
        {
            InitializeComponent();

            ResidentEmailTextBox.Text = person_email;
            ResidentFirstNameTextBox.Text = person_first_name;
            ResidentLastNameTextBox.Text = person_last_name;
            ResidentFatherNameTextBox.Text = person_father_name;
        }
    }
}
