using System.Security.Authentication;
using H4.Day1.Identity.Codes;
using H4.Day1.Identity.Components;
using H4.Day1.Identity.Components.Account;
using H4.Day1.Identity.Data;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AuthenticatedUser", policy =>
    {
        policy.RequireAuthenticatedUser();
    })
    .AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireRole("Admin");
    });

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 8;
});

var todoConnectionString = builder.Configuration.GetConnectionString("TodoDbConnection") ?? throw new InvalidOperationException("Connection string 'TodoDbConnection' not found.");
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseSqlServer(todoConnectionString));

// Ikke nødvendigt da Kestrel automatisk bruger appsettings, secrets etc. så længe paths er sat rigtigt op
// Men beholdt som eksempel.

//builder.WebHost.UseKestrel(((context, options) =>
//{
//    options.Configure(context.Configuration.GetSection("Kestrel"))
//        .Endpoint("HTTPS", listenOptions =>
//    {
//        listenOptions.HttpsOptions.SslProtocols = SslProtocols.Tls13;
//    });
//}));

//string kestrelCertUrl = builder.Configuration.GetValue<string>("KestrelCertUrl")
//    .Replace("%USERPROFILE%", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));

//string kestrelCertPassword = builder.Configuration.GetValue<string>("KestrelCertPassword");

//builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Path").Value = kestrelCertUrl;
//builder.Configuration.GetSection("Kestrel:Endpoints:Https:Certificate:Password").Value = kestrelCertPassword;

builder.Services.AddScoped<SymmetricalEncryption>();
builder.Services.AddScoped<ASymmetricalEncryption>();
builder.Services.AddScoped<HashingHandler>();
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
