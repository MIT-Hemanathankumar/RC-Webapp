using System;
using System.Collections.Generic;
using System.Text;

namespace PDM.Model
{
    public class Contact
    {
        public long AddressId { get; set; }
        public int ContactTypeId { get; set; }
        public string Value { get; set; }
    }
}
