using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels;

public class CreateProductViewModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public Guid MenuId { get; set; }

    [Required]
    [Display(Name = "Choose Image")]
    public IFormFile ImageFile { get; set; } = null!;

    public double Price { get; set; }

    public double? OldPrice { get; set; }
    
    public int? Number { get; set; }
}