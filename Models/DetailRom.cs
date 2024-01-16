using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class DetailRom
    {
        public Guid Id { get; set; }
        public Guid? IdRom { get; set; }
        public Guid? IdProduct { get; set; }
        public double? Price { get; set; }

        public virtual Product? IdProductNavigation { get; set; }
        public virtual Rom? IdRomNavigation { get; set; }
    }
}
