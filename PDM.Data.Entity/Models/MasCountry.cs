using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasCountry
    {
        public MasCountry()
        {
            MasState = new HashSet<MasState>();
            ProAddress = new HashSet<ProAddress>();
        }

        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<MasState> MasState { get; set; }
        public virtual ICollection<ProAddress> ProAddress { get; set; }
    }
}
