using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasOrderType
    {
        public MasOrderType()
        {
            ProOrder = new HashSet<ProOrder>();
        }

        public int OrderTypeId { get; set; }
        public string OrderType { get; set; }

        public virtual ICollection<ProOrder> ProOrder { get; set; }
    }
}
