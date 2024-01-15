using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Color
    {
        public Color()
        {
            DetailColors = new HashSet<DetailColor>();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        

        public virtual ICollection<DetailColor> DetailColors { get; set; }
    }
}
