using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel
{
    public class Resident
    {
        public int person_id;
        public string person_email;
        public string person_first_name;
        public string person_last_name;
        public string person_father_name;

        public Resident(int person_id, string person_email, string person_first_name, string person_last_name, string person_father_name)
        {
            this.person_id = person_id;
            this.person_email = person_email;
            this.person_first_name = person_first_name;
            this.person_last_name = person_last_name;
            this.person_father_name = person_father_name;
        }
    }
}
