using System.ComponentModel.DataAnnotations;

namespace WebBanHang.ViewModels;

public class LoginAdminViewModel
{
    [Required]
    public string UserName { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}