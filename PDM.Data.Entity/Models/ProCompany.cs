using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProCompany
    {
        public ProCompany()
        {
            ProBranch = new HashSet<ProBranch>();
            ProOrder = new HashSet<ProOrder>();
            ProUserMap = new HashSet<ProUserMap>();
        }

        public long CompanyId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string CompanyName { get; set; }
        public string Description { get; set; }
        public string LicenseCode { get; set; }
        public int IsActive { get; set; }

        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual ICollection<ProBranch> ProBranch { get; set; }
        public virtual ICollection<ProOrder> ProOrder { get; set; }
        public virtual ICollection<ProUserMap> ProUserMap { get; set; }
    }
}
