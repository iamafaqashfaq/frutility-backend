using frutility_backend.Data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class OrderCartVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderStatus { get; set; }
        public List<byte[]> ImageBytes { get; set; }
        public virtual Products Products { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
