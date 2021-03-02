using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProUserDocument
    {
        public long UserDocumentId { get; set; }
        public long UserId { get; set; }
        public string Caption { get; set; }
        public string DocumentPath { get; set; }
        public int DocumentType { get; set; }

        public virtual ProUser User { get; set; }
    }
}
