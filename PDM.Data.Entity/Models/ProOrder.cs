using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProOrder
    {
        public ProOrder()
        {
            ProDelivery = new HashSet<ProDelivery>();
            ProOrderDetail = new HashSet<ProOrderDetail>();
        }

        public long OrderId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public long? CompanyId { get; set; }
        public long? BranchId { get; set; }
        public long? OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public long? CustomerId { get; set; }
        public long? CaseSheetId { get; set; }
        public int? OrderStatus { get; set; }
        public int? OrderTypeId { get; set; }
        public string DeliveryNote { get; set; }
        public string BranchNote { get; set; }
        public int? PickupTypeId { get; set; }
        public int? StorageId { get; set; }

        public virtual ProBranch Branch { get; set; }
        public virtual ProCompany Company { get; set; }
        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProCustomer Customer { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual MasOrderType OrderType { get; set; }
        public virtual MasPickupType PickupType { get; set; }
        public virtual ICollection<ProDelivery> ProDelivery { get; set; }
        public virtual ICollection<ProOrderDetail> ProOrderDetail { get; set; }
    }
}
