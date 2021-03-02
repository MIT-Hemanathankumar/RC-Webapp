using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProCaseSheet
    {
        public ProCaseSheet()
        {
            ProCaseSheetDetail = new HashSet<ProCaseSheetDetail>();
        }

        public long CaseSheetId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public long? CustomerId { get; set; }
        public string CaseSheetName { get; set; }
        public string Description { get; set; }
        public long? DoctorId { get; set; }
        public int IsActive { get; set; }

        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProCustomer Customer { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual ICollection<ProCaseSheetDetail> ProCaseSheetDetail { get; set; }
    }
}
