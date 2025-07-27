using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Controllers
{
    [Authorize(Roles = "superadmin, admin")]
    public class AdminController(UserManager<User> userManager, ApplicationDbContext context) : Controller
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ApplicationDbContext _context = context;

        // FIND USERS
        [HttpGet]
        public IActionResult GetUsers(string currentRole)
        {
            ADMINGetUsersViewModel model = new()
            {
                Users = [.. _userManager.GetUsersInRoleAsync(currentRole).Result],
                Role = currentRole
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetUserByParameters(ADMINGetUsersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Something went wrong."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            List<User>? users = null;
            if (model.Id is not null)
            {
                users = [.. _userManager.GetUsersInRoleAsync(model.Role).Result.Where(u => u.Id == model.Id)];
            }
            else if (model.UserName is not null)
            {
                users = [.. _userManager.GetUsersInRoleAsync(model.Role).Result.Where(u => u.UserName == model.UserName)];
            }
            else if (model.Email is not null)
            {
                users = [.. _userManager.GetUsersInRoleAsync(model.Role).Result.Where(u => u.Email == model.Email)];
            }
            ADMINGetUsersViewModel newModel = new()
            {
                Users = users,
                Role = model.Role
            };
            return View("~/Views/Admin/GetUsers.cshtml", newModel);
        }

        // EDIT USER
        [HttpGet]
        public async Task<IActionResult> SetAsAdmin(string userId)
        {
            User? user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The user does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            else
            {
                IList<string>? userRole = _userManager.GetRolesAsync(user!).Result;
                if ((userRole is not null) && !userRole.Contains("admin"))
                {
                    _ = await _userManager.RemoveFromRolesAsync(user!, userRole);
                    _ = await _userManager.AddToRoleAsync(user!, "admin");
                }
                return RedirectToRoute(new { controller = "Admin", action = "GetUsers", currentRole = "admin" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> DeleteUser(string userId, string currentRole)
        {
            List<User> user = [.. _context.Users.Include(u => u.Series).Where(u => u.Id == userId)];
            if (user.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The user does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            else
            {
                if (user[0].Series is not null)
                {
                    foreach (Series work in user[0].Series!)
                    {
                        work.User = null!;
                    }
                }
                IdentityResult result = await _userManager.DeleteAsync(user[0]);
                return RedirectToRoute(new { controller = "Admin", action = "GetUsers", currentRole });
            }
        }

        // VIEW SERIES
        [HttpGet]
        public IActionResult GetSeries()
        {
            List<Series> series = [.. _context.Series];
            List<BasicSeriesData> basicSeriesDatas = [];
            foreach (Series work in series)
            {
                basicSeriesDatas.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
            }
            ADMINGetSeriesViewModel model = new()
            {
                Series = basicSeriesDatas
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult GetSeriesByParameters(ADMINGetSeriesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Something went wrong."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            List<Series> series = [];
            if (model.Id is not null)
            {
                series = [.. _context.Series.Where(u => u.SeriesId == Guid.Parse(model.Id))];
            }
            else if (model.Title is not null)
            {
                series = [.. _context.Series.Where(u => u.Title == model.Title)];
            }
            else if (model.Year is not null)
            {
                series = [.. _context.Series.Where(u => u.Year == model.Year)];
            }
            List<BasicSeriesData> basicSeriesDatas = [];
            foreach (Series work in series)
            {
                basicSeriesDatas.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
            }
            var newModel = new ADMINGetSeriesViewModel()
            {
                Series = basicSeriesDatas
            };
            return View("~/Views/Admin/GetSeries.cshtml", newModel);
        }

        // EDIT SERIES
        [HttpGet]
        public IActionResult ViewProfile(string userId)
        {
            List<User> author = [.. _context.Users.Include(a => a.Series).Where(a => a.Id == userId)];
            if (author.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The user does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            IList<string> authorRole = _userManager.GetRolesAsync(author[0]).Result;
            ADMINViewProfileViewModel model = new()
            {
                ExtendedUserDataPart = new VIEWMyAccountViewModel
                {
                    BasicUserDataPart = new BasicUserData
                    {
                        Id = author[0].Id,
                        Name = author[0].UserName,
                        Bio = author[0].Bio
                    },
                    Email = author[0].Email
                },
                Role = authorRole[0],
                Series = author[0].Series
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult EditSeries(Guid seriesId)
        {
            List<Series> series = [.. _context.Series.Include(s => s.Genre).Include(s => s.User).Include(s => s.Chapters).Where(s => s.SeriesId == seriesId)];
            if (series.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The series does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            return View(series[0]);
        }

        [HttpGet]
        public async Task<ActionResult> DeleteSeries(Guid seriesId)
        {
            List<Series> series = [.. _context.Series.Where(s => s.SeriesId == seriesId)];
            if (series.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The series does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            List<Fave> faves = [.. _context.Faves.Where(f => f.Series == series[0])];
            if (faves.Count > 0)
            {
                foreach (Fave fave in faves)
                {
                    _ = _context.Faves.Remove(fave);
                }
            }
            if ((series[0].FolderLink is not null) && Directory.Exists(series[0].FolderLink))
            {
                Directory.Delete(series[0].FolderLink, recursive: true);
            }
            if ((series[0].CoverLink is not null) && Directory.Exists(series[0].CoverLink))
            {
                System.IO.File.Delete(series[0].CoverLink!);
            }
            _ = _context.Series.Remove(series[0]);
            _ = await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Admin", action = "GetSeries" });
        }
    }
}
