using PDM.Data.Entity;
using PDM.Data.Entity.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Model
{
    public class Address
    {
        public Address()
        {
            if (Contacts == null)
                Contacts = new List<Contact>();
        }
        [DisplayName("Address line 1")]
        public string Address1 { get; set; }


        [DisplayName("Address line 2")]
        public string Address2 { get; set; }

        [DisplayName("Town / City name")]
        public string City { get; set; }

        [DisplayName("State")]
        public int StateId { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        [DisplayName("Post Code")]
        public string PostCode { get; set; }
        public ICollection<Contact> Contacts { get; set; }
    }
}
