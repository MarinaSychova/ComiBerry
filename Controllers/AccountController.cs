using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace ComiBerry.Controllers
{
    public class AccountController(SignInManager<User> signInManager, UserManager<User> userManager) : Controller
    {
        private readonly SignInManager<User> _signInManager = signInManager;
        private readonly UserManager<User> _userManager = userManager;

        [HttpPost]
        public async Task<IActionResult> Login(VIEWLoginPartialViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.FindByEmailAsync(model.Email!);
                if (user is not null)
                {
                    SignInResult singInResult = await _signInManager.PasswordSignInAsync(user.UserName, model.Password!, false, false);
                    if (singInResult.Succeeded)
                    {
                        ViewData["PageName"] = "Explore";
                        return RedirectToAction("Home", "Navigation");
                    }
                }
            }
            return RedirectToAction("Register", "Account");
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(VIEWRegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                User? user = await _userManager.FindByEmailAsync(model.Email!);
                if (user is null)
                {
                    User newUser = new()
                    {
                        UserName = model.Name!,
                        Email = model.Email!
                    };

                    IdentityResult registerResult = await _userManager.CreateAsync(newUser, model.Password!);

                    if (registerResult.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, "user");
                        SignInResult singInResult = await _signInManager.PasswordSignInAsync(newUser.UserName, model.Password!, false, false);
                        if (singInResult.Succeeded)
                        {
                            ViewData["PageName"] = "Explore";
                            return RedirectToAction("Home", "Navigation");
                        }
                    }
                }
            }
            return View(model);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            ViewData["PageName"] = "Explore";
            return RedirectToAction("Home", "Navigation");
        }
    }
}
