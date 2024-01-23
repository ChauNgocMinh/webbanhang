using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels
{
    public class CreateImagesViewModel
    {
        public Guid? IdProduct { get; set; }
        public string? Img { get; set; }

        [Required]
        [Display(Name = "Choose Image")]
        public IFormFile ImageFile { get; set; } = null!;
    }
}
