using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasCategory
    {
        public MasCategory()
        {
            ProProduct = new HashSet<ProProduct>();
        }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int ParentId { get; set; }
        public int IsActive { get; set; }

        public virtual ICollection<ProProduct> ProProduct { get; set; }
    }
}
