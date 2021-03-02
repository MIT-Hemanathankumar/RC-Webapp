using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProBranch
    {
        public ProBranch()
        {
            ProOrder = new HashSet<ProOrder>();
            ProUserMap = new HashSet<ProUserMap>();
        }

        public long BranchId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public long CompanyId { get; set; }
        public string BranchName { get; set; }
        public long AddressId { get; set; }
        public string OrderPrefix { get; set; }
        public string OrderSuffix { get; set; }
        public long NextOrderNo { get; set; }
        public int NoOfRacks { get; set; }
        public int NoOfRows { get; set; }
        public int IsActive { get; set; }

        public virtual ProAddress Address { get; set; }
        public virtual ProCompany Company { get; set; }
        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual ICollection<ProOrder> ProOrder { get; set; }
        public virtual ICollection<ProUserMap> ProUserMap { get; set; }
    }
}
