using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasType
    {
        public MasType()
        {
            ProProduct = new HashSet<ProProduct>();
        }

        public int TypeId { get; set; }
        public string Type { get; set; }

        public virtual ICollection<ProProduct> ProProduct { get; set; }
    }
}
