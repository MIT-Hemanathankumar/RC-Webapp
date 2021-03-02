using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasRole
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int IsActive { get; set; }
    }
}
