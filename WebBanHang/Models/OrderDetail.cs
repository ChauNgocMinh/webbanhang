using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.Models;

namespace WebBanHang.Model
{
    [Table("OrderDetail")]
    public class OrderDetail
    {
        public Guid Id { get; set; }
        public Guid? OrderId { get; set; }
        public Guid ProductId { get; set; }/*
        public string ProductName { get; set; }
        public string ProductImage { get; set; }*/
        public double? ProductPrice { get; set; }
        public int? Qty { get; set; }
        public bool? Status { get; set; }
        public string Email { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
