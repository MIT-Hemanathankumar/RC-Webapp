using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProUser
    {
        public ProUser()
        {
            InverseBranchAdmin = new HashSet<ProUser>();
            InverseCreatedByNavigation = new HashSet<ProUser>();
            InverseModifiedByNavigation = new HashSet<ProUser>();
            ProBranchCreatedByNavigation = new HashSet<ProBranch>();
            ProBranchModifiedByNavigation = new HashSet<ProBranch>();
            ProCaseSheetCreatedByNavigation = new HashSet<ProCaseSheet>();
            ProCaseSheetModifiedByNavigation = new HashSet<ProCaseSheet>();
            ProCompanyCreatedByNavigation = new HashSet<ProCompany>();
            ProCompanyModifiedByNavigation = new HashSet<ProCompany>();
            ProCustomer = new HashSet<ProCustomer>();
            ProDeliveryCreatedByNavigation = new HashSet<ProDelivery>();
            ProDeliveryModifiedByNavigation = new HashSet<ProDelivery>();
            ProOrderCreatedByNavigation = new HashSet<ProOrder>();
            ProOrderModifiedByNavigation = new HashSet<ProOrder>();
            ProProductCreatedByNavigation = new HashSet<ProProduct>();
            ProProductModifiedByNavigation = new HashSet<ProProduct>();
            ProUserDocument = new HashSet<ProUserDocument>();
            ProUserMap = new HashSet<ProUserMap>();
        }

        public long UserId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public long? AddressId { get; set; }
        public int UserTypeId { get; set; }
        public long? BranchAdminId { get; set; }
        public string Comments { get; set; }
        public int IsActive { get; set; }

        public virtual ProAddress Address { get; set; }
        public virtual ProUser BranchAdmin { get; set; }
        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual ProUserType UserType { get; set; }
        public virtual ICollection<ProUser> InverseBranchAdmin { get; set; }
        public virtual ICollection<ProUser> InverseCreatedByNavigation { get; set; }
        public virtual ICollection<ProUser> InverseModifiedByNavigation { get; set; }
        public virtual ICollection<ProBranch> ProBranchCreatedByNavigation { get; set; }
        public virtual ICollection<ProBranch> ProBranchModifiedByNavigation { get; set; }
        public virtual ICollection<ProCaseSheet> ProCaseSheetCreatedByNavigation { get; set; }
        public virtual ICollection<ProCaseSheet> ProCaseSheetModifiedByNavigation { get; set; }
        public virtual ICollection<ProCompany> ProCompanyCreatedByNavigation { get; set; }
        public virtual ICollection<ProCompany> ProCompanyModifiedByNavigation { get; set; }
        public virtual ICollection<ProCustomer> ProCustomer { get; set; }
        public virtual ICollection<ProDelivery> ProDeliveryCreatedByNavigation { get; set; }
        public virtual ICollection<ProDelivery> ProDeliveryModifiedByNavigation { get; set; }
        public virtual ICollection<ProOrder> ProOrderCreatedByNavigation { get; set; }
        public virtual ICollection<ProOrder> ProOrderModifiedByNavigation { get; set; }
        public virtual ICollection<ProProduct> ProProductCreatedByNavigation { get; set; }
        public virtual ICollection<ProProduct> ProProductModifiedByNavigation { get; set; }
        public virtual ICollection<ProUserDocument> ProUserDocument { get; set; }
        public virtual ICollection<ProUserMap> ProUserMap { get; set; }
    }
}
