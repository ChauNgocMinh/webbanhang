using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Migrations.Seeding;
using WebBanHang.Models;

namespace WebBanHang;

public static class AppExtentions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, WebApplicationBuilder builder)
    {
        return builder.Configuration["DatabaseProvider"] switch
        {
            "MsSql" => services.AddDbContext<WebBanHangContext, MsSqlDbContext>(),
            _ => services.AddDbContext<WebBanHangContext, SqliteDbContext>(),
        };
    }

    public static IServiceCollection UseIdentity(this IServiceCollection services)
    {
        services.AddDefaultIdentity<AppUser>(options =>
        {
            // Add options here
        })
        .AddRoles<AppRole>()
        .AddEntityFrameworkStores<WebBanHangContext>()
        .AddSignInManager<SignInManager<AppUser>>();
        return services;
    }

    public static IServiceCollection ConfigIdentity(this IServiceCollection services, WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
            return ConfigIdentityForDevelopment(services);
        return ConfigIdentityForProduction(services);
    }

    private static IServiceCollection ConfigIdentityForDevelopment(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings.
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 1000;
            options.Lockout.AllowedForNewUsers = false;

            // User settings.
            options.User.RequireUniqueEmail = false;
        });

        return ConfigCookieForIdentity(services, 99999);
    }

    private static IServiceCollection ConfigIdentityForProduction(IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Configure more if needed

            // Password settings.
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 6;
            options.Password.RequiredUniqueChars = 1;

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings.
            options.User.AllowedUserNameCharacters =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            options.User.RequireUniqueEmail = false;
        });

        return ConfigCookieForIdentity(services, 100);
    }

    private static IServiceCollection ConfigCookieForIdentity(IServiceCollection services, double expireMinutes = 5)
    {
        services.ConfigureApplicationCookie(options =>
        {
            // Cookie settings
            options.Cookie.HttpOnly = true;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(expireMinutes);

            // TODO: change path here
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
            options.SlidingExpiration = true;
        });
        return services;
    }

    public async static Task ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<WebBanHangContext>();

        context = app.Configuration["DatabaseProvider"] switch
        {
            "MsSql" => services.GetRequiredService<MsSqlDbContext>(),
            _ => services.GetRequiredService<SqliteDbContext>(),
        };

        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            logger.LogError("An error occured during migration with error details below: {}", ex.Message);
            logger.LogError("{}", ex.StackTrace);
        }
    }

    public async static Task SeedAppData(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        var logger = services.GetRequiredService<ILogger<Program>>();

        try
        {
            await DataSeeding.CreateRolesAsync(roleManager);
            await DataSeeding.CreateAppAdminAsync(userManager);
        }
        catch (Exception ex)
        {
            logger.LogError("An error occured during seeding with error details below: {}", ex.Message);
            logger.LogError("{}", ex.StackTrace);
        }
    }
}