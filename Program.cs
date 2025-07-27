global using Microsoft.AspNetCore.Identity;
global using Microsoft.EntityFrameworkCore;
global using ComiBerry.Services;
global using ComiBerry.Data;
global using ComiBerry.ViewModels;
global using ComiBerry.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<EncryptionService>();
builder.Services.AddControllersWithViews();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Domain = "localhost"; // Set the domain for the cookie
    options.Cookie.Path = "/"; // Cookie is available within the entire application\
    options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Ensure the cookie is only sent over HTTPS (set to false for local development)
    options.Cookie.HttpOnly = true; // Prevent client-side scripts from accessing the cookie
    options.Cookie.IsEssential = true; // Indicates the cookie is essential for the application to function
    options.LoginPath = "/Navigation/Home";
    options.AccessDeniedPath = "/Navigation/Home";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseMiddleware<RequireValidUserMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Navigation}/{action=Home}/{id?}");

/*using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new List<string> { "superadmin", "admin", "user" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    string name = "SuperAdmin";
    string email = "superadmin@gmail.com";
    string password = "SuperAdmin0";
}*/

app.Run();

