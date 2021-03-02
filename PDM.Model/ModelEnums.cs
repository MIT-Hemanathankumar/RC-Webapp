using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PDM.Model
{
    public enum ContactTypes
    {
        ContactPerson = 1,
        Mobile = 2,
        LandLine = 3,
        Email = 4,
        AlternativeContact = 5,
        DependentContactNumber = 6
    }

    public enum AddressType
    {
        DeliveryAddress = 1,
        BillingAddress = 2
    }

}
