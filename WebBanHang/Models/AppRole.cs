using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Models;

public class AppRole : IdentityRole
{
    public List<AppUserRole> UserRoles { get; set; } = new();
}