using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasStrength
    {
        public MasStrength()
        {
            ProProduct = new HashSet<ProProduct>();
        }

        public int StrengthId { get; set; }
        public string Strength { get; set; }

        public virtual ICollection<ProProduct> ProProduct { get; set; }
    }
}
