using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProProduct
    {
        public ProProduct()
        {
            ProOrderDetail = new HashSet<ProOrderDetail>();
        }

        public long ProductId { get; set; }
        public DateTime CreatedOn { get; set; }
        public long CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public int CategoryId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int IsActive { get; set; }
        public int StrengthId { get; set; }
        public int TypeId { get; set; }
        public int UomId { get; set; }

        public virtual MasCategory Category { get; set; }
        public virtual ProUser CreatedByNavigation { get; set; }
        public virtual ProUser ModifiedByNavigation { get; set; }
        public virtual MasStrength Strength { get; set; }
        public virtual MasType Type { get; set; }
        public virtual MasUom Uom { get; set; }
        public virtual ICollection<ProOrderDetail> ProOrderDetail { get; set; }
    }
}
