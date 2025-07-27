using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Controllers
{
    public class ReaderController(ApplicationDbContext context) : Controller
    {
        private readonly ApplicationDbContext _context = context;
        
        [HttpGet]
        public IActionResult ViewSeries(Guid seriesId)
        {
            List<Series> series = [.. _context.Series.Include(s => s.Genre).Include(s => s.User).Include(s => s.Chapters).Where(s => s.SeriesId == seriesId)];
            if (series.IsNullOrEmpty() || !series[0].IsVisible)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The series does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            IQueryable<Fave> likes = _context.Faves.Where(l => l.Series == series[0]);
            ExtendedSeriesData model = new()
            {   BasicSeriesDataPart = new BasicSeriesData
                {
                    Id = series[0].SeriesId,
                    Title = series[0].Title,
                    Year = series[0].Year
                },
                Language = series[0].Language,
                AgeRestriction = series[0].AgeRestriction,
                Layout = series[0].Layout,
                Direction = series[0].Direction,
                Description = series[0].Description,
                Tags = series[0].Tags,
                ViewCount = series[0].ViewCount,
                Genres = [.. series[0].Genre],
                UserId = series[0].User is null ? string.Empty : series[0].User!.Id,
                UserName = series[0].User is null ? string.Empty : series[0].User!.UserName,
                LikeCount = likes.Count(),
                Chapters = series[0].Chapters.IsNullOrEmpty() ? [] : [.. series[0].Chapters!]
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult ViewAuthor(string authorId)
        {
            List<User> author = [.. _context.Users.Include(a => a.Series).Where(a => a.Id == authorId)];
            if (author.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The user does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            VIEWViewAuthorViewModel model = new()
            {
                BasicUserDataPart = new BasicUserData
                {
                    Id = author[0].Id,
                    Name = author[0].UserName,
                    Bio = author[0].Bio
                },
                Series = author[0].Series.IsNullOrEmpty() ? [] : [.. author[0].Series!.Where(s => s.IsVisible)]
            };
            return View(model);
        }
        
        [HttpGet]
        public async Task<IActionResult> ViewChapter(Guid chapterId, int chapterIndex, string layout, string direction)
        {
            List<Chapter> chapter = [.. _context.Chapters.Include(c => c.Pages).Include(c => c.Series).Include(c => c.Series.User).Where(x => x.ChapterId == chapterId)];
            if (chapter.IsNullOrEmpty() || !chapter[0].IsPublished)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "The chapter does not exist."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            User? user = HttpContext.Items["CurrentUser"] as User;
            List<Comment> comments = [.. _context.Comments.Include(c => c.User).Where(c => c.Chapter == chapter[0])];
            if ((user is not null) && (user != chapter[0].Series.User))
            {
                chapter[0].Series.ViewCount += 1;
                _ = await _context.SaveChangesAsync();
            }

            VIEWViewChapterViewModel model = new()
            {
                BasicChapterDataPart = new BasicChapterData
                {
                    Pages = [.. chapter[0].Pages],
                    Layout = layout,
                    Direction = direction,
                    FirstPageId = chapter[0].FirstPage
                },
                ChapterId = chapter[0].ChapterId,
                ChapterIndex = chapterIndex,
                Title = chapter[0].Title,
                SeriesTitle = chapter[0].Series.Title,
                Comments = comments
            };
            return View(model);
        }

        public class LikeRequest
        {
            public required string SeriesId { get; set; }
        }
        
        [Authorize]
        public async Task<IActionResult> LikeSeries([FromBody] LikeRequest request)
        {
            if (HttpContext.Items["CurrentUser"] is not User user) { return Json(new { success = false }); }

            List<Series> series = [.. _context.Series.Where(s => s.SeriesId == Guid.Parse(request.SeriesId))];
            if (series[0].User == user) { return Json(new { success = false }); }

            List<Fave> existingFave = [.. _context.Faves.Where(f => (f.User == user) && (f.Series == series[0]))];
            if (existingFave.IsNullOrEmpty())
            {
                Fave newFave = new()
                {
                    User = user,
                    Series = series[0]
                };
                _ = _context.Faves.Add(newFave);
                _ = await _context.SaveChangesAsync();
                int likes = _context.Faves.Where(l => l.Series == series[0]).Count();
                return Json(new { success = true, likeCount = likes });
            }
            return Json(new { success = false });
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddComment(Guid chapterId, string text)
        {
            User? user = HttpContext.Items["CurrentUser"] as User;
            Comment comment = new()
            {
                Text = text,
                DateTime = DateTime.Now,
                Chapter = _context.Chapters.First(c => c.ChapterId == chapterId),
                User = user!
            };
            _ = _context.Comments.Add(comment);
            _ = await _context.SaveChangesAsync();
            return HttpContext.Request.Headers.TryGetValue("Referer", out Microsoft.Extensions.Primitives.StringValues refererUrl)
                ? Redirect(refererUrl.ToString())
                : RedirectToAction("Home", "Navigation");
        }
        
        [HttpGet]
        public IActionResult GetSeriesByGenres(List<string> genres, bool andOperation)
        {
            List<Genre> allGenres = [.. _context.Genres];
            List<Genre> chosenGenres = [];
            foreach (string index in genres)
            {
                chosenGenres.Add(_context.Genres.First(g => g.GenreId == int.Parse(index)));
            }
            List<BasicSeriesData> seriesList = [];

            foreach (Genre genre in chosenGenres)
            {
                IQueryable<Series> series = _context.Series.Include(s => s.Genre).Where(s => s.Genre.Contains(genre));
                if (series.Any())
                {
                    foreach (Series work in series)
                    {
                        if (!seriesList.Any(w => w.Id == work.SeriesId) && work.IsVisible)
                        {
                            if (andOperation)
                            {
                                List<Genre> seriesGenres = [.. work.Genre];
                                if (seriesGenres.Count != 0 && !seriesGenres.Except(chosenGenres).Any() && !chosenGenres.Except(seriesGenres).Any())
                                {
                                    seriesList.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
                                }
                            }
                            else
                            {
                                seriesList.Add(new BasicSeriesData { Id = work.SeriesId, Title = work.Title, Year = work.Year });
                            }
                        }
                    }
                }
            }

            SeriesListViewModel searchModel = new()
            {
                Series = seriesList
            };
            ViewData["PageName"] = "Explore";
            return View("~/Views/Navigation/Home.cshtml", searchModel);
        }
    }
}
