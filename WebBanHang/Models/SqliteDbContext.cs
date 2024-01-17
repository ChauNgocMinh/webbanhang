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

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}