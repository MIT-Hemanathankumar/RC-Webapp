using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProOrderDetail
    {
        public long OrderDetailId { get; set; }
        public long? OrderId { get; set; }
        public long? ProductId { get; set; }
        public int? Morning { get; set; }
        public int? AfterNoon { get; set; }
        public int? Evening { get; set; }
        public string Remarks { get; set; }
        public string Duration { get; set; }
        public decimal? Quantity { get; set; }

        public virtual ProOrder Order { get; set; }
        public virtual ProProduct Product { get; set; }
    }
}
