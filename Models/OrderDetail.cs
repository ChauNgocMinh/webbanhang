using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid ProductId { get; set; }
        public double? ProductPrice { get; set; }
        public int? Qty { get; set; }
        public bool? Status { get; set; }
        public string? Email { get; set; }

        public virtual Order? Order { get; set; }
        public virtual Product Product { get; set; } = null!;
    }
}
