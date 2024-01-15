using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class DetailColor
    {
        public Guid? IdProduct { get; set; }
        public Guid? IdColor { get; set; }
        public Guid Id { get; set; }
        public double? Price { get; set; }
        public virtual Color? IdColorNavigation { get; set; }
        public virtual Product? IdProductNavigation { get; set; }
    }
}
