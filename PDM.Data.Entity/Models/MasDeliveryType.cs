using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasDeliveryType
    {
        public MasDeliveryType()
        {
            ProDelivery = new HashSet<ProDelivery>();
        }

        public int DeliveryTypeId { get; set; }
        public string DeliveryType { get; set; }

        public virtual ICollection<ProDelivery> ProDelivery { get; set; }
    }
}
