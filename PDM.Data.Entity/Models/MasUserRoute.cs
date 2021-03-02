using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasUserRoute
    {
        public int RouteId { get; set; }
        public string RouteName { get; set; }
        public int IsActive { get; set; }
    }
}
