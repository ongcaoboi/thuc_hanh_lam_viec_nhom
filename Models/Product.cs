using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class Product
    {
        public Product()
        {
            Galleries = new HashSet<Gallery>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int IdCategory { get; set; }
        public string Title { get; set; } = null!;
        public int Price { get; set; }
        public string Thumbnail { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Quantity { get; set; }

        public virtual Category IdCategoryNavigation { get; set; } = null!;
        public virtual ICollection<Gallery> Galleries { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
