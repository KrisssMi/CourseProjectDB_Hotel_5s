using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Rent
    {
        public int inventory_type_id;
        public string inventory_type_name;
        public int inventory_id;
        public string inventory_description;
        public float inventory_daily_price;

        public List<DateTime> selectedList = new List<DateTime>();

        public Rent(int inventory_type_id, string inventory_type_name, int inventory_id, string inventory_description, float inventory_daily_price, List<DateTime> selectedList)
        {
            this.inventory_type_id = inventory_type_id;
            this.inventory_type_name = inventory_type_name;
            this.inventory_id = inventory_id;
            this.inventory_description = inventory_description;
            this.inventory_daily_price = inventory_daily_price;

            this.selectedList = selectedList;
        }
    }
}
