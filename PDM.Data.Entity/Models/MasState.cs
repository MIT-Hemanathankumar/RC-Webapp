using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasState
    {
        public MasState()
        {
            ProAddress = new HashSet<ProAddress>();
        }

        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string StateName { get; set; }
        public string StateCode { get; set; }
        public int IsActive { get; set; }

        public virtual MasCountry Country { get; set; }
        public virtual ICollection<ProAddress> ProAddress { get; set; }
    }
}
