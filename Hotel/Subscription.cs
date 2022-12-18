using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Subscription
    {
        public int service_type_id;
        public string service_type_name;
        public float service_type_daily_price;
        public int staff_id;
        public string staff_email;
        public int service_id;

        public List<DateTime> selectedList = new List<DateTime>();

        public Subscription(int service_type_id, string service_type_name, float service_type_daily_price, int staff_id, string staff_email, int service_id, List<DateTime> selectedList)
        {
            this.service_type_id = service_type_id;
            this.service_type_name = service_type_name;
            this.service_type_daily_price = service_type_daily_price;
            this.staff_id = staff_id;
            this.staff_email = staff_email;
            this.service_id = service_id;

            this.selectedList = selectedList;
        }
    }
}
