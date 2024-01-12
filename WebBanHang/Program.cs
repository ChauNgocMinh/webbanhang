using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Identity.Data;
using WebBanHang.Models;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("WebBanHangContextConnection") ?? throw new InvalidOperationException("Connection string 'WebBanHangContextConnection' not found.");

builder.Services.AddDbContext<WebBanHangContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<WebBanHangUser>(/*options => options.SignIn.RequireConfirmedAccount = true*/)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<WebBanHangContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();


// Add session 
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60*24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
