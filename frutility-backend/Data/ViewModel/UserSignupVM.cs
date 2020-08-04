using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class UserSignupVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCity{ get; set; }
        public string BillingAddress { get; set; }
        public string BillingState { get; set; }
        public string BillingCity { get; set; }
        public string Phone { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
