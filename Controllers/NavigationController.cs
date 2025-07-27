using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ComiBerry.Controllers
{
    public class NavigationController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        
        [Authorize]
        [AjaxOnly]
        [HttpGet]
        public PartialViewResult MenuPartial()
        {
            User user = (User)HttpContext.Items["CurrentUser"]!;
            VIEWUserMenuViewModel menuModel = new()
            {
                Id = user!.Id,
                Name = user!.UserName
            };
            return PartialView("~/Views/Shared/_UserMenuPartial.cshtml", menuModel);
        }

        [Authorize(Roles = "superadmin, admin")]
        [AjaxOnly]
        [HttpGet]
        public PartialViewResult AdminMenuPartial()
        {
            return PartialView("~/Views/Shared/_AdminMenuPartial.cshtml");
        }

        [AjaxOnly]
        [HttpGet]
        public PartialViewResult LoginPartial()
        {
            return PartialView("~/Views/Shared/_LoginPartial.cshtml");
        }

        [AjaxOnly]
        [HttpGet]
        public PartialViewResult SearchPartial()
        {
            GenreListViewModel model = new()
            { 
                Genres = [.. _context.Genres]
            };
            return PartialView("~/Views/Shared/_SearchPartial.cshtml", model);
        }

        [HttpGet]
        public ActionResult Home()
        {
            List<Series> series = [.. _context.Series.Where(s => s.IsVisible)];
            List<BasicSeriesData> basicSeriesDatas = [];
            foreach (Series work in series)
            {
                basicSeriesDatas.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
            }
            SeriesListViewModel worksModel = new()
            {
                Series = basicSeriesDatas
            };
            ViewData["PageName"] = "Explore";
            return View(worksModel);
        }
    }
}
