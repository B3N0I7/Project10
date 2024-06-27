using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MLS_UI.Data;
using MLS_UI.Interfaces;
using MLS_UI.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add logging
builder.Logging.ClearProviders().AddConsole();

// Add HttpClient
builder.Services.AddHttpClient();

// Add database connection
//var connectionString = builder.Configuration.GetConnectionString("Connection");
//builder.Services.AddDbContext<UserDbContext>(options =>
//    options.UseSqlServer(connectionString));

var server = Environment.GetEnvironmentVariable("DatabaseServer");
var port = Environment.GetEnvironmentVariable("DatabasePort");
var user = Environment.GetEnvironmentVariable("DatabaseUser");
var password = Environment.GetEnvironmentVariable("DatabasePassword");
var database = Environment.GetEnvironmentVariable("DatabaseName");

var connectionString = $"Server={server}, {port}; Initial Catalog = {database}; User ID = {user}; password ={password}; TrustServerCertificate=True";

builder.Services.AddDbContext<UserDbContext>(option => option.UseSqlServer(connectionString));

//, sqlServerOptionsAction: sqlOptions =>
//{
//    sqlOptions.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
//}));

// Add identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// Add interface
builder.Services.AddScoped<IAuthentInterface, AuthentInterface>();

// Add authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// ajout options cookie
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(180); // Durée de validité du cookie
        options.SlidingExpiration = true; // Renouvelle le cookie à chaque requête si l'utilisateur est actif
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,      // avant true
            ValidateAudience = false,       // avant true
            RequireExpirationTime = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetSection("JwtConfig:Issuer").Value,
            ValidAudience = builder.Configuration.GetSection("JwtConfig:Audience").Value,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("JwtConfig:Secret").Value))
        };
    });
// Ajout services pour cookie-token
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

// Add Razor Pages
//builder.Services.AddRazorPages();

// Add View Components
//builder.Services.AddRazorComponents();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Add database initialisation
DatabaseManagementService.MigrationInitialisation(app);

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

// ajout app session
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authent}/{action=Login}/{id?}");

//app.UseEndpoints(endpoints =>
//{
//    endpoints.MapControllerRoute(
//        name: "default",
//        pattern: "{controller=Authent}/{action=Login}/{id?}");

//    //endpoints.MapRazorComponents();

//    endpoints.MapControllers();
//});

app.Run();
