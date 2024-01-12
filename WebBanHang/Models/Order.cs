using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBanHang.Model
{
    [Table("Order")]
    public class Order
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double? TotalAmount { get; set; }
        public int Status { get; set; }
        public string Note { get; set; }
        public DateTime Created { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
