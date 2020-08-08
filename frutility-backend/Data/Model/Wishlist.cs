using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.Model
{
    public class Wishlist
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("ApplicationUser")]
        public string UserId { get; set; }
        [ForeignKey("Products")]
        public int ProductId { get; set; }
        public DateTime PostingDate { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Products Products { get; set; }
    }
}
