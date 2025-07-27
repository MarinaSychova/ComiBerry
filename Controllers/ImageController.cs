using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Controllers
{
    public class ImageController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;

        public IActionResult GetAvatar(string userId)
        {
            List<User> user = [.. _context.Users.Where(u => u.Id == userId)];
            string filePath = !user.IsNullOrEmpty() && System.IO.File.Exists(user[0].AvatarLink)
                ? user[0].AvatarLink!
                : Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\defav.jpg");
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "image/png");
        }
        
        public IActionResult GetCoverImage(Guid seriesId)
        {
            User? user = HttpContext.Items["CurrentUser"] as User;
            List<Series> series = [.. _context.Series.Where(s => s.SeriesId == seriesId)];
            string filePath = !series.IsNullOrEmpty()
                && (series[0].IsVisible || ((user is not null) && (series[0].User == user)))
                && System.IO.File.Exists(series[0].CoverLink)
                ? filePath = series[0].CoverLink!
                : filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\defcover.jpg");
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "image/png");
        }
        
        public IActionResult GetPageImage(Guid pageId)
        {
            User? user = HttpContext.Items["CurrentUser"] as User;
            List<Page> page = [.. _context.Pages.Include(p => p.Chapter).ThenInclude(c => c.Series).Where(p => p.PageId == pageId)];
            string filePath = !page.IsNullOrEmpty()
                && (page[0].Chapter.IsPublished || ((user is not null) && (page[0].Chapter.Series.User == user)))
                && System.IO.File.Exists(page[0].PageLink)
                ? filePath = page[0].PageLink
                : filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\defcover.jpg");
            FileStream fileStream = System.IO.File.OpenRead(filePath);
            return File(fileStream, "image/png");
        }
    }
}
