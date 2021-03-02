using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace PDM.Model
{
    public class Base
    {
        public List<SelectListItem> PickupTypes { get; set; }
        public List<SelectListItem> States { get; set; }
        public List<SelectListItem> Countries { get; set; }
        public List<SelectListItem> Routes { get; set; }

        public long CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}
