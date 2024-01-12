using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Menu
    {
        public Menu()
        {
            Products = new HashSet<Product>();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
