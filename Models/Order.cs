using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? IdUser { get; set; }
        public int? IdStatus { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public string? Note { get; set; }
        public DateTime OrderDate { get; set; }

        public virtual Status? IdStatusNavigation { get; set; }
        public virtual User? IdUserNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
