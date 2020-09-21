using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class ProductsGetVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public double Price { get; set; }
        public double? PriceBeforeDiscount { get; set; }
        public string imageURI { get; set; }
        public double ShippingCharges { get; set; }
        public bool Availability { get; set; }
        public int Stock { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime UpdationDate { get; set; }
        public double PackageWeight { get; set; }
        public int SubCategoryID { get; set; }
    }
}
