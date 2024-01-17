using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Models;

public class AppUserRole : IdentityUserRole<string>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public AppUser User { get; set; } = null!;
    public AppRole Role { get; set; } = null!;
}