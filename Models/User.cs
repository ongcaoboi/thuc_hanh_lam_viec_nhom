using System;
using System.Collections.Generic;

namespace web_bh.Models
{
    public partial class User
    {
        public User()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public int IdRole { get; set; }
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string DisName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Avatar { get; set; }

        public virtual Role IdRoleNavigation { get; set; } = null!;
        public virtual ICollection<Order> Orders { get; set; }
    }
}
