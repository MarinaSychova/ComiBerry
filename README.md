<p align="center">
  <img width="600" height="200" src="wwwroot/images/logo-color.svg">
</p>

# ComiBerry
A simple platform for comics publication. This is my university bachelor project which can be used as a template for a much bigger project.

## Features
  - profile settings (change avatar, e-mail and password)
  - upload comics and edit them later
  - file types: JPG, PNG, GIF
  - mark series as favourites
  - leave comments under chapters
  - admin moderation: view lists of users, delete them, upgrade them to admins, view their series

## Tools used
  - ASP.NET Core 
  - SQL Server
  - HTML, CSS, JavaScript, AJAX

![](preview.gif)

## How to set up the Visual Studio project
### 1 Create a new project
Download all the folders, Program.cs and appsettings.json files of this project. In your Visual Studio, create new “ASP.NET Core Empty” project and call it “ComiBerry”. Paste all the downloaded files and folders into your new ComiBerry project.
### 2 Download NuGet files
> [!NOTE]
> The version of these packages used in this project is 8.0.11 for .NET 8. Download the latest versions for your current .NET version.
  - Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
  - Microsoft.AspNetCore.Identity.EntityFrameworkCore
  - Microsoft.AspNetCore.Identity.UI
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Tools
### 3 Connect to your database
1) Create a new database with Microsoft SQL Server Management Studio.
2) Open appsettings.json inside your ComiBerry project and find the “DefaultConnection” string and replace ```{YourServer}``` and ```{YourDatabase}``` with the name of your server and database respectively (curly brackets are not needed).
3) Replace ```{AvatarsFolderPath}```, ```{CoversFolderPath}```, and ```{SeriesFolderPath}``` with the path to the folders where avatars of users, covers of series and folders of series will be stored respectively (curly brackets are not needed here too).
### 4 Update your database
In Visual Studio, go Tools > NuGet Package Manager > Package Manager Console. In the console, write a command: ```update-database```. All the migrations from the Migrations folder will be applied to your database which was created in the previous step.
### 5 Add roles to the database
Open Program.cs file inside your ComiBerry project. Paste this code before the 'app.Run();' line:
```
using (var scope = app.Services.CreateScope())
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
}
```
This will create the roles the users can get, you can view them in the AspNetRoles table in your database. Delete this code after the first run of the project, it is not needed anymore.
### 6 Create a super admin
To create a super admin user, paste this code before the “app.Run();” line in the Program.cs file too:
```
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    User superAdmin = new()
    {
        UserName = "SuperAdmin",
        Email = "superadmin@gmail.com"
    };
    IdentityResult registerResult = await userManager.CreateAsync(superAdmin, "SuperAdmin0");
    if (registerResult.Succeeded)
    {
        await userManager.AddToRoleAsync(superAdmin, "superadmin");
    }
}
```
It will create a super admin with a user name “SuperAdmin”, a “superadmin@gmail.com” e-mail, and a “SuperAdmin0” password. You can change these values to your liking. Do not forget to also delete this code after the first run. Now you should be able to log in as a super admin or create another account.
## Is contribution allowed?
Yes, of course. Just create a new brach if you want to add a new functionality or fix an issue.
