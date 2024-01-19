using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Migrations.Seeding;
using WebBanHang.Models;

namespace WebBanHang;

public static class AppExtentions
{
    public static IServiceCollection AddAppDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        return configuration.GetValue<string>("DatabaseProvider") switch
        {
            "Sqlite" => services.AddDbContext<WebBanHangContext, SqliteDbContext>(),
            _ => services.AddDbContext<WebBanHangContext, MsSqlDbContext>(),
        };
    }

    public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIdentity<AppUser, AppRole>(options =>
        {
            // Add options here
            options.SignIn.RequireConfirmedAccount = configuration.GetSection("Identity:Options:SignIn").GetValue<bool>("RequireConfirmedAccount");
        })
            .AddEntityFrameworkStores<WebBanHangContext>()
            .AddDefaultTokenProviders();

        services.Configure<IdentityOptions>(options =>
        {
            // Configure more if needed

            var passwordSettingsConfig = configuration.GetSection("Identity:Options:Password");

            // Password settings.
            options.Password.RequireDigit = passwordSettingsConfig.GetValue<bool>("RequireDigit");
            options.Password.RequireLowercase = passwordSettingsConfig.GetValue<bool>("RequireLowercase");
            options.Password.RequireNonAlphanumeric = passwordSettingsConfig.GetValue<bool>("RequireNonAlphanumeric");
            options.Password.RequireUppercase = passwordSettingsConfig.GetValue<bool>("RequireUppercase");
            options.Password.RequiredLength = passwordSettingsConfig.GetValue<int>("RequiredLength");
            options.Password.RequiredUniqueChars = passwordSettingsConfig.GetValue<int>("RequiredUniqueChars");

            var lockoutSettingsConfig = configuration.GetSection("Identity:Options:Lockout");

            // Lockout settings.
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(lockoutSettingsConfig.GetValue<double>("DefaultLockoutTimeSpanMinutes"));
            options.Lockout.MaxFailedAccessAttempts = lockoutSettingsConfig.GetValue<int>("MaxFailedAccessAttempts");
            options.Lockout.AllowedForNewUsers = lockoutSettingsConfig.GetValue<bool>("AllowedForNewUsers");

            var userSettingsConfig = configuration.GetSection("Identity:Options:User");

            // User settings.
            options.User.AllowedUserNameCharacters = userSettingsConfig.GetValue<string>("AllowedUserNameCharacters");
            options.User.RequireUniqueEmail = userSettingsConfig.GetValue<bool>("RequireUniqueEmail");
        });

        services.ConfigureApplicationCookie(options =>
        {
            var cookieSettingsConfig = configuration.GetSection("Identity:Cookie");
            // Settings for cookie authentication
            options.Cookie.HttpOnly = cookieSettingsConfig.GetValue<bool>("HttpOnly");
            options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieSettingsConfig.GetValue<double>("ExpireMinutes"));

            options.LoginPath = "/Admin/Login";
            options.LogoutPath = "/Admin/Logout";
            options.AccessDeniedPath = "/Admin/AccessDenied";
            options.SlidingExpiration = true;
        });

        return services;
    }

    public async static Task ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<WebBanHangContext>();

        context = app.Configuration.GetValue<string>("DatabaseProvider") switch
        {
            "Sqlite" => services.GetRequiredService<SqliteDbContext>(),
            _ => services.GetRequiredService<MsSqlDbContext>(),
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

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("Only System Admin Role", policy =>
            {
                policy.RequireRole("System Admin");
            });
        });
        return services;
    }

    public static IServiceCollection InjectAppServices(this IServiceCollection services)
    {
        services.AddScoped<PaymentServices>();
        return services;
    }

}