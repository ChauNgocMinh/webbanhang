using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace WebBanHang.Models;

// https://dev.to/moesmp/ef-core-multiple-database-providers-3gb7
public abstract class WebBanHangContext : IdentityDbContext<AppUser>
{
    protected readonly IConfiguration Configuration;

    protected WebBanHangContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Color> Colors { get; set; }
    public DbSet<DetailColor> DetailColors { get; set; }
    public DbSet<DetailRom> DetailRoms { get; set; }
    public DbSet<Menu> Menus { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetail> OrderDetails { get; set; }
    public DbSet<Rom> Roms { get; set; }
}
