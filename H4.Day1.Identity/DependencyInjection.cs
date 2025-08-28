using H4.Day1.Identity.Codes;
using H4.Day1.Identity.Components.Account;
using H4.Day1.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace H4.Day1.Identity;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ApplicationDbConnection")
                               ?? throw new InvalidOperationException("Connection string 'ApplicationDbConnection' not found.");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString,
                o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "AppDb")));

        services.AddDatabaseDeveloperPageExceptionFilter();

        return services;
    }

    public static IServiceCollection AddTodoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var todoConnectionString = configuration.GetConnectionString("TodoDbConnection")
                                   ?? throw new InvalidOperationException("Connection string 'TodoDbConnection' not found.");

        services.AddDbContext<TodoDbContext>(options => options.UseSqlServer(todoConnectionString,
            o => o.MigrationsHistoryTable(HistoryRepository.DefaultTableName, "TodoDb")));

        return services;
    }

    public static IServiceCollection AddIdentityServices(this IServiceCollection services)
    {
        services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
        });

        return services;
    }

    public static IServiceCollection AddEncryptionServices(this IServiceCollection services)
    {
        services.AddScoped<SymmetricalEncryption>();
        services.AddScoped<ASymmetricalEncryption>();
        services.AddScoped<HashingHandler>();
        return services;
    }

    public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy("AuthenticatedUser", policy =>
            {
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("RequireAdminRole", policy =>
            {
                policy.RequireRole("Admin");
            });
        return services;
    }

    // Ikke nødvendigt da Kestrel automatisk bruger AppSettings, Secrets etc. så længe paths er sat rigtigt op
    // Men beholdt som eksempel.
    /*
    public static void ConfigureKestrel(WebApplicationBuilder builder)
    {
        builder.WebHost.UseKestrel(((context, options) =>
        {
            options.Configure(context.Configuration.GetSection("Kestrel"))
                .Endpoint("HTTPS", listenOptions =>
            {
                listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls13;
            });
        }));

        string kestrelCertUrl = builder.Configuration.GetValue<string>("KestrelCertUrl")
            .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

        string kestrelCertPassword = builder.Configuration.GetValue<string>("KestrelCertPassword");

        builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Path").Value = kestrelCertUrl;
        builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Password").Value = kestrelCertPassword;
    }
    */
}