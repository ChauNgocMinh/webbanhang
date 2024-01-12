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
            //BillDetails = new HashSet<BillDetail>();
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid? MenuId { get; set; }
        public string? Image { get; set; }

        public double Price { get; set; }
        public int? Number { get; set; }

        public virtual Menu? Menu { get; set; }
        //public virtual ICollection<BillDetail> BillDetails { get; set; }

        [NotMapped]
        [Display(Name = "Choose Image")]
        public IFormFile ImageFile { get; set; }
    }
}
