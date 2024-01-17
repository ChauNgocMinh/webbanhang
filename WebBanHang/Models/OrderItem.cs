using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class OrderItem
    {
        public Guid Id { get; set; }
        public Guid? ProductId { get; set; }
        public double? Price { get; set; }
        public int? Quantity { get; set; }
        public string ProductName { get; set; } = null!;
        public string Image { get; set; } = null!;
        public Guid? InfoOrderId { get; set; }

        public virtual InfoOrder? InfoOrder { get; set; }
    }
}
