﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class WishlistGetVM
    {
        public int Id { get; set; }
        public int productId { get; set; }
        public string ProductName { get; set; }
        public double ProductPrice { get; set; }
        public List<byte[]> ImageBytes { get; set; }
    }
}
