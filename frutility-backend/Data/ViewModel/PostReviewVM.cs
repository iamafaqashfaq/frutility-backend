﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace frutility_backend.Data.ViewModel
{
    public class PostReviewVM
    {
        public int ProductId { get; set; }
        public int Quality { get; set; }
        public string Review { get; set; }
    }
}
