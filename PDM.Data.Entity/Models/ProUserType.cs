using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProUserType
    {
        public ProUserType()
        {
            ProUser = new HashSet<ProUser>();
        }

        public int UserTypeId { get; set; }
        public string UserType { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ProUser> ProUser { get; set; }
    }
}
