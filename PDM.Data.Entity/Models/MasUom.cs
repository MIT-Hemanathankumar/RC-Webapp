using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasUom
    {
        public MasUom()
        {
            ProProduct = new HashSet<ProProduct>();
        }

        public int UomId { get; set; }
        public string Uom { get; set; }

        public virtual ICollection<ProProduct> ProProduct { get; set; }
    }
}
