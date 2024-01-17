using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels;

public class CreateUserViewModel
{
    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = null!;

    [Required]
    [DataType(DataType.Text)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = null!;

    [Required]
    [Display(Name = "Tên Tài Khoản")]
    [StringLength(255, MinimumLength = 6)]
    public string UserName { get; set; } = null!;

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required]
    public string Role { get; set; } = null!;
}