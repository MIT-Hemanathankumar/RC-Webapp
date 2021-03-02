using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProDelivery
    {
        public long DeliveryId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public long? OrderId { get; set; }
        public bool? IsRefrigiration { get; set; }
        public long? DriverId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public int? DeliveryTypeId { get; set; }
        public int? DeliveryStatus { get; set; }

        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual MasDeliveryType DeliveryType { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual ProOrder Order { get; set; }
    }
}
