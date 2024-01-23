using System.ComponentModel.DataAnnotations;
using WebBanHang.Models;

namespace WebBanHang.ViewModels;

public class EditProductViewModel
{
    [Required]
    public Guid ProductId { get; set; }
    
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public Guid MenuId { get; set; }

    public string? OldImage { get; set; }

    [Display(Name = "Choose Image")]
    public IFormFile? ImageFile { get; set; }

    public double Price { get; set; }

    public double? OldPrice { get; set; }
    
    public int? Number { get; set; }

    public List<Image> Images { get; set; }
    public List<Rom> rom { get; set; }
    public List<Color> color { get; set; }
}