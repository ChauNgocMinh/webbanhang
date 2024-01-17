using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;

namespace WebBanHang.Migrations.Seeding;

public class DataSeeding
{
    public static async Task CreateRolesAsync(RoleManager<AppRole> roleManager)
    {
        if (await roleManager.Roles.AnyAsync()) return;

        var roles = new List<string>()
        {
            "System Admin",
            "Sale",
            "Sale Admin"
        };

        foreach (var roleName in roles)
        {
            await roleManager.CreateAsync(new AppRole
            {
                Name = roleName
            });
        }
    }

    public static async Task CreateAppAdminAsync(UserManager<AppUser> userManager)
    {
        var admin = await userManager.FindByNameAsync("admin");

        if (admin != null) return;

        admin = new AppUser
        {
            FirstName = "Administrator",
            LastName = "System",
            UserName = "admin",
        };

        await userManager.CreateAsync(admin, "P@ssw0rd");

        await userManager.AddToRoleAsync(admin, "System Admin");
    }
}