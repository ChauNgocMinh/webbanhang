
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Models;

public class SqliteDbContext : WebBanHangContext
{
    public SqliteDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite(Configuration.GetConnectionString("SqliteConn"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Color>(entity =>
        {
            entity.ToTable("Color");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .IsFixedLength();
        });

        modelBuilder.Entity<DetailColor>(entity =>
        {
            entity.ToTable("DetailColor");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.IdProductNavigation)
                .WithMany(p => p.DetailColors)
                .HasForeignKey(d => d.IdProduct)
                .HasConstraintName("FK_DetailRom_Product");

            entity.HasOne(d => d.IdColorNavigation)
                .WithMany(p => p.DetailColors)
                .HasForeignKey(d => d.IdColor)
                .HasConstraintName("FK_DetailRom_Color");
        });

        modelBuilder.Entity<DetailRom>(entity =>
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

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.ToTable("Menu");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.ToTable("Order");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Created).HasPrecision(1);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.ToTable("OrderDetail");

            entity.HasIndex(e => e.OrderId, "IX_OrderDetail_OrderId");

            entity.HasIndex(e => e.ProductId, "IX_OrderDetail_ProductId");

            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.Property(e => e.Email).HasMaxLength(50);

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
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

        modelBuilder.Entity<Rom>(entity =>
        {
            entity.ToTable("Rom");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

    }
}