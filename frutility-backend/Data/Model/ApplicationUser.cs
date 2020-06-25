using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string ShippingAddress { get; set; }
        public string ShippingState { get; set; }
        public string ShippingCity { get; set; }
        public int ShippingPincode { get; set; }
        public string BillingAddress { get; set; }
        public string BillingState { get; set; }
        public string BillingCity { get; set; }
        public int BillingPincode { get; set; }
        public DateTime RegDate { get; set; }
        public DateTime UpdationDate { get; set; }
    }
}
