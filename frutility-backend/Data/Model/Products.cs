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
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductVendor { get; set; }
        public double Price { get; set; }
        public double ProductPriceBeforeDiscount { get; set; }
        public string ProductImage1 { get; set; }
        public string ProductImage2 { get; set; }
        public string ProductImage3 { get; set; }
        public double ShippingCharges { get; set; }
        public bool ProductAvailability { get; set; }
        public int ProductStock { get; set; }
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
