using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class InfoOrder
    {
        public Guid Id { get; set; }
        public int? Status { get; set; }
        public bool? PaymentMethod { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? District { get; set; }
        public string? Address { get; set; }
        public float? Total { get; set; }
        public DateTime? Date { get; set; }

        public virtual ICollection<OrderItem> OrderItems { get; set; } = null!;
    }
}
