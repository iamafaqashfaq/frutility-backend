using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class ChangeAddressVM
    {
        public string ShippingAddress { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCity { get; set; }
        public string BillingAddress{ get; set; }
        public string BillingState { get; set; }
        public string BillingCity { get; set; }
    }
}
