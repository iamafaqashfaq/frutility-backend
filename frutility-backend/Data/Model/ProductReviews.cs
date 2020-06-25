using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.Model
{
    public class ProductReviews
    {
        public int Id { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public int Quality { get; set; }
        public int Price { get; set; }
        public int Value { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Review { get; set; }
        public DateTime ReviewDate { get; set; }
        public virtual Products Products { get; set; }
    }
}
