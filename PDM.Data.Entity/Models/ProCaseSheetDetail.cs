using System;
using System.Collections.Generic;

namespace PDM.Data.Entity.Models
{
    public partial class ProCaseSheetDetail
    {
        public long CaseSheetDetailId { get; set; }
        public long CaseSheetId { get; set; }
        public string Caption { get; set; }
        public string DocumentPath { get; set; }
        public int DocumentType { get; set; }

        public virtual ProCaseSheet CaseSheet { get; set; }
    }
}
