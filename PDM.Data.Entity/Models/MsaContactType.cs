using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MsaContactType
    {
        public MsaContactType()
        {
            ProContact = new HashSet<ProContact>();
        }

        public int ContactTypeId { get; set; }
        public string ContactType { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ProContact> ProContact { get; set; }
    }
}
