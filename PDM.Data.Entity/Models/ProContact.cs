using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProContact
    {
        public long AddressId { get; set; }
        public int ContactTypeId { get; set; }
        public string Value { get; set; }

        public virtual ProAddress Address { get; set; }
        public virtual MsaContactType ContactType { get; set; }
    }
}
