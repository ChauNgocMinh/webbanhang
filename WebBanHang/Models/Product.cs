using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models;

public class Product
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public Guid MenuId { get; set; }
    public string? Image { get; set; }
    public double Price { get; set; }
    public int? Number { get; set; }
    public Guid? IdRom { get; set; }
    public double? OldPrice { get; set; }
    public DateTime? Created { get; set; }
    [NotMapped]
    [Display(Name = "Choose Image")]
    public IFormFile ImageFile { get; set; } = null!;
    public virtual Menu? Menu { get; set; }
    public HashSet<DetailColor> DetailColors { get; set; } = new();
    public HashSet<DetailRom> DetailRoms { get; set; } = new();
    public List<OrderDetail> OrderDetails { get; set; } = new();
}
