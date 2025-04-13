using Assignment01.Areas.Management.Models;
using Assignment01.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddDbContext<Assignment01DB>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
Log.Information("Starting up the application");


builder.Services.AddDefaultIdentity<Account>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<Assignment01DB>();


// Add MVC

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

var app = builder.Build();

//Global error handling
app.UseExceptionHandler("/Home/Error");
app.UseStatusCodePagesWithReExecute("/Home/Error", "?code={0}");

app.Use(async (context, next) =>
{
    var user = context.User.Identity.IsAuthenticated ? context.User.Identity.Name : "Anonymous";
    var endpoint = context.Request.Path;

    Log.Information("User {User} accessed endpoint {Endpoint} at {Timestamp}", user, endpoint, DateTime.UtcNow);

    await next();
});

// seed roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Account>>();
    var email = "NewAdmin@gmail.com";
    var password = "123456";
    var user = await userManager.FindByEmailAsync(email);

    if (user == null)
    {
        user = new Account
        {
            UserName = email,
            Email = email,
            FirstName = "Admin",
            LastName = "Admin",
            EmailConfirmed = true,
            Address = email,
            CreatedAt = DateTime.UtcNow
        };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded) await userManager.AddToRoleAsync(user, "Admin");
    }
}


if (!app.Environment.IsDevelopment()) app.UseHsts();


app.UseHttpsRedirection();
app.UseRouting();

// Configure the HTTP request pipeline.
app.UseAuthentication(); // Enable authentication
app.UseAuthorization(); // Enable authorization
app.MapStaticAssets();


app.MapControllerRoute(
    "home",
    "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();


app.Run();