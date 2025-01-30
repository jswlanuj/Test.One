using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using Test.One.Data;
using Test.One.Models;
using Test.One.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<UsersServices>();

// Configure Google authentication settings
builder.Services.Configure<GoogleClientSettings>(builder.Configuration.GetSection("GoogleClient"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Users/Login";  // Redirect here if the user is not logged in
    options.AccessDeniedPath = "/Users/AccessDenied"; // Optional: when access is denied
    options.ExpireTimeSpan = TimeSpan.FromDays(30); // Adjust session timeout as needed
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["GoogleClient:ClientId"];
    options.ClientSecret = builder.Configuration["GoogleClient:ClientSecret"];
    options.Scope.Add("email"); // Ensure email scope is added to get user's email
    options.SaveTokens = true; // Save the tokens to the session to retrieve later
    options.CallbackPath = new Microsoft.AspNetCore.Http.PathString("/Users/GoogleCallback");  // Set the callback URI
});

// Add session and other required services
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Enable session middleware
app.UseSession();

// Configure the default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");

app.Run();
