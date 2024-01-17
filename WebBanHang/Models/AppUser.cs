using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Models;

public class AppUser : IdentityUser
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public List<AppUserRole> UserRoles { get; set; } = new();
}