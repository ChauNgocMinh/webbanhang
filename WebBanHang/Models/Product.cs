using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public partial class Product
    {
        public Product()
        {
            DetailColors = new HashSet<DetailColor>();
            DetailRoms = new HashSet<DetailRom>();
            OrderDetails = new HashSet<OrderDetail>();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? MenuId { get; set; }
        public string? Image { get; set; }
        public double Price { get; set; }
        public int? Number { get; set; }
        public Guid? IdRom { get; set; }
        public double? OldPrice { get; set; }
        public DateTime? Created { get; set; }
        [NotMapped]
        [Display(Name = "Choose Image")]
        public IFormFile ImageFile { get; set; }
        public virtual Menu? Menu { get; set; }
        public virtual ICollection<DetailColor> DetailColors { get; set; }
        public virtual ICollection<DetailRom> DetailRoms { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
