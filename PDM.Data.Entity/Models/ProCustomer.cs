using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProCustomer
    {
        public ProCustomer()
        {
            ProCaseSheet = new HashSet<ProCaseSheet>();
            ProOrder = new HashSet<ProOrder>();
        }

        public long CustomerId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string Nhsnumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public long? AddressId { get; set; }
        public string Gender { get; set; }
        public DateTime Dob { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime? NextOrderDate { get; set; }
        public int? PickupTypeId { get; set; }
        public int IsActive { get; set; }
        public string Title { get; set; }

        public virtual ProAddress Address { get; set; }
        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ICollection<ProCaseSheet> ProCaseSheet { get; set; }
        public virtual ICollection<ProOrder> ProOrder { get; set; }
    }
}
