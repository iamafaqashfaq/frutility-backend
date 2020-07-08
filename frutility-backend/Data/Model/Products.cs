using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.Model
{
    public class Products
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Vendor { get; set; }
        public double Price { get; set; }
        public double? PriceBeforeDiscount { get; set; }
        public string Image1 { get; set; }
        public string Image2 { get; set; }
        public string Image3 { get; set; }
        public double ShippingCharges { get; set; }
        public bool Availability { get; set; }
        public int Stock { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime UpdataionDate { get; set; }
        public double PackageWeight { get; set; }
        [ForeignKey("SubCategory")]
        public int SubCategoryID { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        [ForeignKey("Category")]
        public int CategoryID { get; set; }
        public virtual Category Category { get; set; }
    }
}
