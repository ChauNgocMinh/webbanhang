using System;
using System.Collections.Generic;

namespace WebBanHang.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public string Address { get; set; } = null!;
        public double? TotalAmount { get; set; }
        public int Status { get; set; }
        public string Note { get; set; } = null!;
        public DateTime Created { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
