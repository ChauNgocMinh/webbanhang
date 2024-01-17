using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

#nullable disable

namespace WebBanHang.Models;

// https://dev.to/moesmp/ef-core-multiple-database-providers-3gb7
public abstract class WebBanHangContext : IdentityDbContext<AppUser, AppRole, string, IdentityUserClaim<string>, AppUserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
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
    public DbSet<Rom> Roms { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public virtual DbSet<InfoOrder> InfoOrders { get; set; } = null!;
    public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Color>(entity =>
        {
            entity.ToTable("Color");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        builder.Entity<DetailColor>(entity =>
        {
            entity.ToTable("DetailColor");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdProductNavigation)
                .WithMany(p => p.DetailColors)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_DetailColor_Product");

            entity.HasOne(d => d.IdColorNavigation)
                .WithMany(p => p.DetailColors)
                .HasForeignKey(d => d.IdColor)
                .HasConstraintName("FK_DetailColor_Color");
        });

        builder.Entity<DetailRom>(entity =>
        {
            entity.ToTable("DetailRom");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdProductNavigation)
                .WithMany(p => p.DetailRoms)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_DetailRom_Product");

            entity.HasOne(d => d.IdRomNavigation)
                .WithMany(p => p.DetailRoms)
                .HasForeignKey(d => d.IdRom)
                .HasConstraintName("FK_DetailRom_Rom");
        });

        builder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        
        builder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.HasIndex(e => e.MenuId, "IX_Product_MenuId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Created).HasColumnType("datetime");

            entity.HasOne(d => d.Menu)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("fk_Product_Menu");
        });

        builder.Entity<Rom>(entity =>
        {
            entity.ToTable("Rom");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        builder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItem");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.InfoOrder)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.InfoOrderId)
                .HasConstraintName("FK__OrderItem__Info");
        });

        builder.Entity<InfoOrder>(entity =>
        {
            entity.ToTable("InfoOrder");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Address).HasMaxLength(150);

            entity.Property(e => e.City).HasMaxLength(50);

            entity.Property(e => e.District).HasMaxLength(50);

            entity.Property(e => e.Email).HasMaxLength(100);

            entity.Property(e => e.Name).HasMaxLength(100);

            entity.Property(e => e.Phone).HasMaxLength(10);
        });

        builder.Entity<AppUser>()
            .HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.Entity<AppRole>()
            .HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .IsRequired();

        builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        builder.Entity<AppUserRole>().ToTable("UserRoles");
        builder.Entity<AppRole>().ToTable("Roles");
        builder.Entity<AppUser>().ToTable("Users");
    }

}
