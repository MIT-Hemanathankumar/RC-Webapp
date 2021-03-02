using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasPickupType
    {
        public MasPickupType()
        {
            ProOrder = new HashSet<ProOrder>();
        }

        public int PickupTypeId { get; set; }
        public string PickupType { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ProOrder> ProOrder { get; set; }
    }
}
