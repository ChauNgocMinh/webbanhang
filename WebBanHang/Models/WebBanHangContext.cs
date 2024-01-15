using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WebBanHang.Areas.Identity.Data;

namespace WebBanHang.Models
{
    public partial class WebBanHangContext : IdentityDbContext<WebBanHangUser>
    {
        public WebBanHangContext()
        {
        }

        public WebBanHangContext(DbContextOptions<WebBanHangContext> options)
            : base(options)
        {
        }

       
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<DetailColor> DetailColors { get; set; } = null!;
        public virtual DbSet<DetailRom> DetailRoms { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Rom> Roms { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=112.78.2.32;Initial Catalog=mag13134_WebBanHang;User ID=mag13134_mag13134;Password=Hihi1234567!;TrustServerCertificate=True;Persist Security Info=True;");
            }
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

                entity.HasOne(d => d.IdColorNavigation)
                    .WithMany(p => p.DetailColors)
                    .HasForeignKey(d => d.IdColor)
                    .HasConstraintName("FK_DetailColor_Color");

                entity.HasOne(d => d.IdProductNavigation)
                    .WithMany(p => p.DetailColors)
                    .HasForeignKey(d => d.IdProduct)
                    .HasConstraintName("FK_DetailColor_Product");
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
