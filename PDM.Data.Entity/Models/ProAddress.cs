using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProAddress
    {
        public ProAddress()
        {
            ProBranch = new HashSet<ProBranch>();
            ProContact = new HashSet<ProContact>();
            ProCustomer = new HashSet<ProCustomer>();
            ProUser = new HashSet<ProUser>();
        }

        public long AddressId { get; set; }
        public string Description { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public int AddressTypeId { get; set; }
        public string PostCode { get; set; }

        public virtual MasAddressType AddressType { get; set; }
        public virtual MasCountry Country { get; set; }
        public virtual MasState State { get; set; }
        public virtual ICollection<ProBranch> ProBranch { get; set; }
        public virtual ICollection<ProContact> ProContact { get; set; }
        public virtual ICollection<ProCustomer> ProCustomer { get; set; }
        public virtual ICollection<ProUser> ProUser { get; set; }
    }
}
