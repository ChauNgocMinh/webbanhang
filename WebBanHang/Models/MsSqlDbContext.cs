using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Models;

public class MsSqlDbContext : WebBanHangContext
{
    public MsSqlDbContext(IConfiguration configuration) : base(configuration)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlServer(Configuration.GetConnectionString("MsSqlConn"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}