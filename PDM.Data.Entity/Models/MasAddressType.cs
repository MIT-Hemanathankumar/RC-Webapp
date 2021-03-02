using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasAddressType
    {
        public MasAddressType()
        {
            ProAddress = new HashSet<ProAddress>();
        }

        public int AddressTypeId { get; set; }
        public string AddressType { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ProAddress> ProAddress { get; set; }
    }
}
