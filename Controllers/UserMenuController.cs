using System.Collections.Immutable;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ComiBerry.Controllers
{
    [Authorize]
    public class UserMenuController(IConfiguration configuration, SignInManager<User> signInManager, UserManager<User> userManager, ApplicationDbContext context) : Controller
    {
        public readonly IConfiguration _configuration = configuration;
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly UserManager<User> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;
        
        // MY ACCOUNT
        [HttpGet]
        public ActionResult MyAccount()
        {
            User user = (User)HttpContext.Items["CurrentUser"]!;
            VIEWMyAccountViewModel model = new()
            {
                BasicUserDataPart = new BasicUserData
                {
                    Id = user!.Id,
                    Name = user.UserName,
                    Bio = user.Bio
                },
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeAvatar(IFormFile newAvatar)
        {
            if (ModelState.IsValid)
            {
                string[] permittedExtensions = [".jpg", ".png", ".gif"];
                string extension = Path.GetExtension(newAvatar.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    VIEWErrorViewModel errorModel0 = new()
                    {
                        Message = "The file extension must be either JPG, PNG, or GIF."
                    };
                    return View("~/Views/Shared/Error.cshtml", errorModel0);
                }

                string mimeType = newAvatar.ContentType;
                string[] permittedMimeTypes = ["image/jpeg", "image/png", "image/gif"];
                if (!permittedMimeTypes.Contains(mimeType))
                {
                    VIEWErrorViewModel errorModel0 = new()
                    {
                        Message = "The file extension must be either JPG, PNG, or GIF."
                    };
                    return View("~/Views/Shared/Error.cshtml", errorModel0);
                }

                User user = (User)HttpContext.Items["CurrentUser"]!;
                string fileName = user!.Id + extension;
                string filePath = Path.Combine(_configuration.GetConnectionString("Avatars")!, fileName);
                using (FileStream stream = new(filePath, FileMode.Create, FileAccess.Write))
                {
                    await newAvatar.CopyToAsync(stream);
                }
                user.AvatarLink = filePath;
                _ = await _userManager.UpdateAsync(user);
                return RedirectToAction("MyAccount", "UserMenu");
            }
            VIEWErrorViewModel errorModel = new()
            {
                Message = "Try again more carefully."
            };
            return View("~/Views/Shared/Error.cshtml", errorModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> SaveChanges(string name, string email) {
            if (ModelState.IsValid)
            {
                User user = (User)HttpContext.Items["CurrentUser"]!;
                user.UserName = name;
                user.Email = email;
                IdentityResult result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    VIEWErrorViewModel errorModel0 = new()
                    {
                        Message = result.Errors.First().Description
                    };
                    return View("~/Views/Shared/Error.cshtml", errorModel0);
                }
                return RedirectToAction("MyAccount", "UserMenu");
            }
            VIEWErrorViewModel errorModel = new()
            {
                Message = "Try again more carefully."
            };
            return View("~/Views/Shared/Error.cshtml", errorModel);
        }
        
        [HttpPost]
        public async Task<IActionResult> ChangePassword(VIEWMyAccountPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = (User)HttpContext.Items["CurrentUser"]!;
                IdentityResult changeResult = await _userManager.ChangePasswordAsync(user!, model.CurrentPassword!, model.NewPassword!);
                if (changeResult.Succeeded)
                {
                    await _signInManager.SignOutAsync();
                    SignInResult singInResult = await _signInManager.PasswordSignInAsync(user!.UserName, model.NewPassword!, false, false);
                    if (singInResult.Succeeded)
                    {
                        return RedirectToAction("MyAccount", "UserMenu");
                    }
                }
                return RedirectToAction("Home", "Navigation");
            }
            VIEWErrorViewModel errorModel = new()
            {
                Message = "Try again more carefully."
            };
            return View("~/Views/Shared/Error.cshtml", errorModel);
        }

        [HttpPost]
        public async Task<IActionResult> ChangeBio(string bio)
        {
            if (ModelState.IsValid)
            {
                User user = (User)HttpContext.Items["CurrentUser"]!;
                user.Bio = bio;
                _ = await _userManager.UpdateAsync(user);
                return RedirectToAction("MyAccount", "UserMenu");
            }
            VIEWErrorViewModel errorModel = new()
            {
                Message = "Try again more carefully."
            };
            return View("~/Views/Shared/Error.cshtml", errorModel);
        }
        
        // MY WORKS
        [HttpGet]
        public ActionResult MyWorks()
        {
            User user = (User)HttpContext.Items["CurrentUser"]!;
            List<Series> series = [.. _context.Series.Where(s => s.User == user)];
            List<BasicSeriesData> basicSeriesDatas = [];
            foreach (Series work in series)
            {
                basicSeriesDatas.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
            }
            SeriesListViewModel worksModel = new()
            {
                Series = basicSeriesDatas
            };
            return View(worksModel);
        }

        // MY FAVORITES
        [HttpGet]
        public IActionResult MyFaves()
        {
            User user = (User)HttpContext.Items["CurrentUser"]!;
            List<Series> series = [.. _context.Faves.Where(f => f.User == user).Include(f => f.Series).Select(f => f.Series)];
            List<BasicSeriesData> basicSeriesDatas = [];
            foreach (Series work in series)
            {
                basicSeriesDatas.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
            }
            SeriesListViewModel favesModel = new()
            {
                Series = basicSeriesDatas
            };
            ViewData["PageName"] = "My Favorites";
            return View("~/Views/Navigation/Home.cshtml", favesModel);
        }
    }
}
