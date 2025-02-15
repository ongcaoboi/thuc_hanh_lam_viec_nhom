﻿using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public int? IdOrder { get; set; }
        public int? IdProduct { get; set; }
        public int? Num { get; set; }
        public int? TotalMoney { get; set; }

        public virtual Order? IdOrderNavigation { get; set; }
        public virtual Product? IdProductNavigation { get; set; }
    }
}
