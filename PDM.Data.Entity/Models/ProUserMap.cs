using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProUserMap
    {
        public long UserId { get; set; }
        public long CustomerId { get; set; }
        public long BranchId { get; set; }
        public long CompanyId { get; set; }

        public virtual ProBranch Branch { get; set; }
        public virtual ProCompany Company { get; set; }
        public virtual ProUser User { get; set; }
    }
}
