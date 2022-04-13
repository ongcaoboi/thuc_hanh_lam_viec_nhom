using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class Status
    {
        public Status()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Status1 { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; }
    }
}
