using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class MasUserDocumentType
    {
        public int DocumentTypeId { get; set; }
        public string DocumentName { get; set; }
        public int IsActive { get; set; }
    }
}
