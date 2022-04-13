using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class Gallery
    {
        public int Id { get; set; }
        public int IdProduct { get; set; }
        public string Thumbnail { get; set; } = null!;

        public virtual Product IdProductNavigation { get; set; } = null!;
    }
}
