using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ComiBerry.Controllers
{
    [Authorize]
    public class AuthorController(IConfiguration configuration, ApplicationDbContext context) : Controller
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ApplicationDbContext _context = context;

        [HttpGet]
        public IActionResult CreateSeries()
        {
            VIEWCreateSeriesViewModel model = new()
            {
                Genres = [.. _context.Genres]
            };
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateSeries(VIEWCreateSeriesViewModel model)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Try again more carefully."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            User user = (User)HttpContext.Items["CurrentUser"]!;
            Series series = new()
            {
                Title = model.Title!,
                Genre = [_context.Genres.First(g => g.GenreId == model.Genre!)],
                Language = model.Language!,
                Year = DateTime.Now.Year,
                AgeRestriction = model.AgeRestriction!,
                Layout = model.Layout!,
                Direction = model.Direction!,
                AccessMode = model.AccessMode!,
                FreeChapters = model.FreeChapters,
                IsVisible = model.IsVisible,
                User = user,
                FolderLink = ""
            };
            _ = await _context.Series.AddAsync(series);
            string folderLink = Path.Combine(_configuration.GetConnectionString("Series")!, series.SeriesId.ToString());
            _ = Directory.CreateDirectory(folderLink);
            series.FolderLink = folderLink;
            _ = await _context.SaveChangesAsync();
            return RedirectToAction("MyWorks", "UserMenu");
        }

        [AuthorOnly]
        [HttpGet]
        public IActionResult EditSeries(Guid seriesId)
        {
            Series series = _context.Series.Include(s => s.Genre).Include(s => s.Chapters).First(x => x.SeriesId == seriesId);
            VIEWEditSeriesViewModel model = new()
            {
                ExtendedSeriesDataPart = new ExtendedSeriesData
                {
                    BasicSeriesDataPart = new BasicSeriesData
                    {
                        Id = series.SeriesId,
                        Title = series.Title,
                        Year = series.Year
                    },
                    Language = series.Language,
                    AgeRestriction = series.AgeRestriction,
                    Layout = series.Layout,
                    Direction = series.Direction,
                    Description = series.Description,
                    Tags = series.Tags,
                    ViewCount = series.ViewCount,
                    Genres = [.. series.Genre],
                    UserId = series.User is null ? string.Empty : series.User!.Id,
                    UserName = series.User is null ? string.Empty : series.User!.UserName,
                    Chapters = series.Chapters.IsNullOrEmpty() ? [] : [.. series.Chapters!]
                },
                IsVisible = series.IsVisible,
                Genres = [.. _context.Genres]
            };
            return View(model);
        }

        [AuthorOnlyForm]
        [HttpPost]
        public async Task<IActionResult> ChangeCover(Guid seriesId, IFormFile newCover)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Try again more carefully."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            User user = (User)HttpContext.Items["CurrentUser"]!;
            Series series = _context.Series.First(x => x.SeriesId == seriesId);

            string[] permittedExtensions = [".jpg", ".png", ".gif"];
            string extension = Path.GetExtension(newCover.FileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension)) { return RedirectToAction("MyWorks", "UserMenu"); }

            string mimeType = newCover.ContentType;
            string[] permittedMimeTypes = ["image/jpeg", "image/png", "image/gif"];
            if (!permittedMimeTypes.Contains(mimeType)) { return RedirectToAction("MyWorks", "UserMenu"); }

            string fileName = series.SeriesId + extension;
            string directory = _configuration.GetConnectionString("Covers")!;
            string filePath = Path.Combine(directory, fileName);
            using (FileStream stream = new(filePath, FileMode.Create, FileAccess.Write))
            {
                await newCover.CopyToAsync(stream);
            }

            series.CoverLink = filePath;
            _ = await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnlyForm]
        [HttpPost]
        public async Task<IActionResult> SaveGenres(Guid seriesId, List<string> GenresChosen)
        {
            if (GenresChosen is null)
            {
                return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
            }

            User user = (User)HttpContext.Items["CurrentUser"]!;
            Series series = _context.Series.Include(s => s.Genre).First(x => x.SeriesId == seriesId);

            foreach (Genre genre in series.Genre)
            {
                if (!GenresChosen.Contains(genre.GenreId.ToString()))
                {
                    _ = series.Genre.Remove(genre);
                }
            }

            List<Genre> allGenres = [.. _context.Genres];
            foreach (string genreId in GenresChosen)
            {
                if (!series.Genre.Contains(allGenres.First(s => s.GenreId == int.Parse(genreId))))
                {
                    series.Genre.Add(allGenres.First(g => g.GenreId == int.Parse(genreId)));
                }
            }
            _ = await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnly]
        [HttpGet]
        public async Task<IActionResult> MakeVisible(Guid seriesId)
        {
            Series series = _context.Series.Include(s => s.Chapters).First(x => x.SeriesId == seriesId);
            if (!series.IsVisible)
            {
                series.IsVisible = true;
                if (!series.Chapters.IsNullOrEmpty())
                {
                    foreach (Chapter chapter in series.Chapters!)
                    {
                        chapter.IsPublished = true;
                    }
                }
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId = seriesId.ToString() });
        }

        [AuthorOnlyForm]
        [HttpPost]
        public async Task<IActionResult> SaveDescAndTags(Guid seriesId, string description, string tags)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Try again more carefully."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            User user = (User)HttpContext.Items["CurrentUser"]!;
            Series series = _context.Series.First(x => x.SeriesId == seriesId);
            series.Description = description;
            series.Tags = tags;
            _ = await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnly]
        [HttpGet]
        public async Task<IActionResult> DeleteSeries(Guid seriesId)
        {
            Series series = _context.Series.Single(s => s.SeriesId == seriesId);
            if (!series.IsVisible)
            {
                if ((series.FolderLink is not null) && Directory.Exists(series.FolderLink))
                {
                    Directory.Delete(series.FolderLink, recursive: true);
                }
                if (series.CoverLink is not null)
                {
                    System.IO.File.Delete(series.CoverLink);
                }
                _ = _context.Series.Remove(series);
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToAction("MyWorks", "UserMenu");
        }

        [AuthorOnly]
        [HttpGet]
        public IActionResult AddChapter(Guid seriesId)
        {
            if (ModelState.IsValid)
            {
                SeriesIdViewModel addChapterModel = new()
                {
                    SeriesId = seriesId
                };
                return View(addChapterModel);
            }
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnlyForm]
        [HttpPost]
        public async Task<IActionResult> AddChapter(Guid seriesId, string title, ICollection<IFormFile> pages)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Try again more carefully."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            if (pages.IsNullOrEmpty())
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "No pages uploaded."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }
            User user = (User)HttpContext.Items["CurrentUser"]!;
            Series series = _context.Series.Include(s => s.Genre).Include(s => s.Chapters).First(x => x.SeriesId == seriesId);

            Chapter chapter = new()
            {
                Title = title!,
                Date = DateTime.Now,
                FolderLink = "",
                Series = series,
                Pages = []
            };

            _ = await _context.Chapters.AddAsync(chapter);
            string folderLink = Path.Combine(series.FolderLink, chapter.ChapterId.ToString());
            _ = Directory.CreateDirectory(folderLink);
            chapter.FolderLink = folderLink;

            Dictionary<Guid, Page> pagesInTable = [];
            Guid prevGuid = Guid.Empty;

            for (int i = pages!.Count - 1; i >= 0; i--)
            {
                string filePath = Path.Combine(chapter.FolderLink, pages.ElementAt(i).FileName);
                using (FileStream stream = new(filePath, FileMode.Create, FileAccess.Write))
                {
                    await pages.ElementAt(i).CopyToAsync(stream);
                }

                Page page = new()
                {
                    PageLink = filePath,
                    Chapter = chapter
                };

                if (prevGuid != Guid.Empty)
                {
                    page.NextPage = pagesInTable[prevGuid];
                }

                _ = await _context.Pages.AddAsync(page);
                pagesInTable[page.PageId] = page;
                prevGuid = page.PageId;

                if (i == 0)
                {
                    chapter.FirstPage = page.PageId;
                }
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnly]
        [HttpGet]
        public IActionResult EditChapter(Guid seriesId, Guid chapterId)
        {
            Chapter chapter = _context.Chapters.Include(c => c.Series).Include(c => c.Pages).First(x => x.ChapterId == chapterId);
            List<Page> pages = [.. chapter.Pages!];
            VIEWEditChapterViewModel model = new()
            {
                SeriesId = seriesId,
                ChapterId = chapterId,
                Title = chapter.Title,
                Pages = pages,
                FirstPageId = chapter.FirstPage
            };
            return View(model);
        }

        [AuthorOnlyForm]
        [HttpPost]
        public async Task<IActionResult> EditChapter(Guid seriesId, Guid chapterId, VIEWEditChapterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                VIEWErrorViewModel errorModel = new()
                {
                    Message = "Try again more carefully."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel);
            }

            int newPageCount = model.PagesData!.Count(p => p.PageId == Guid.Empty);
            int newPageFileCount = model.NewPages is null ? 0 : model.NewPages.Count;
            if (newPageCount != newPageFileCount)
            {
                VIEWErrorViewModel errorModel0 = new()
                {
                    Message = "Not enough files uploaded."
                };
                return View("~/Views/Shared/Error.cshtml", errorModel0);
            }

            User user = (User)HttpContext.Items["CurrentUser"]!;
            Chapter chapter = _context.Chapters.Include(c => c.Series).First(x => x.ChapterId == chapterId);

            //Delete pages if needed
            for (int i = 0; i < model.PagesData!.Count; i++)
            {
                Guid? currentPageNextPageId = model.PagesData[i].NextPageId;
                if (currentPageNextPageId is null)
                {
                    continue;
                }
                if (!model.PagesData.Any(p => p.PageId == currentPageNextPageId))
                {
                    Page pageToDelete = _context.Pages.First(p => p.PageId == currentPageNextPageId);
                    pageToDelete.NextPage = null;
                    List<Page> pageToDeletePrev = [.. _context.Pages.Where(p => p.PageId == model.PagesData[i].PageId)];
                    if (!pageToDeletePrev.IsNullOrEmpty())
                    {
                        pageToDeletePrev[0].NextPage = null;
                    }
                    _ = _context.Pages.Remove(pageToDelete);
                }
            }

            if (!model.PagesData.Any(p => p.PageId == chapter.FirstPage))
            {
                Page pageToDelete = _context.Pages.First(p => p.PageId == chapter.FirstPage);
                _ = _context.Pages.Remove(pageToDelete);
            }
            _ = await _context.SaveChangesAsync();

            // Add new pages if needed
            if (model.NewPages is not null)
            {
                for (int i = 0; i < model.PagesData.Count; i++)
                {
                    PageData currentPage = model.PagesData[i];
                    if (currentPage.PageId == Guid.Empty)
                    {
                        string filePath = Path.Combine(chapter.FolderLink, model.NewPages.ElementAt(0).FileName);
                        using (FileStream stream = new(filePath, FileMode.Create, FileAccess.Write))
                        {
                            await model.NewPages.ElementAt(0).CopyToAsync(stream);
                        }
                        model.NewPages.RemoveAt(0);

                        Page page = new()
                        {
                            PageLink = filePath,
                            Chapter = chapter
                        };
                        _ = await _context.Pages.AddAsync(page);

                        model.PagesData[i].PageId = page.PageId;
                    }
                }
            }
            _ = await _context.SaveChangesAsync();

            // Switch pages' places if needed
            for (int i = 0; i < model.PagesData.Count - 1; i++)
            {
                PageData currentPage = model.PagesData[i];
                PageData nextPage = model.PagesData[i + 1];
                if (currentPage.NextPageId != nextPage.PageId)
                {
                    Page contextCurrentPage = _context.Pages.First(p => p.PageId == currentPage.PageId);
                    Page contextNextPage = _context.Pages.First(p => p.PageId == nextPage.PageId);
                    contextCurrentPage.NextPage = contextNextPage;
                }
            }

            Page lastPage = _context.Pages.First(p => p.PageId == model.PagesData.Last().PageId);
            lastPage.NextPage = null;

            chapter.FirstPage = (Guid)model.PagesData[0].PageId!;
            _ = await _context.SaveChangesAsync();
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }

        [AuthorOnly]
        [HttpGet]
        public IActionResult ReadChapter(Guid seriesId, Guid chapterId, string layout, string direction)
        {
            Chapter chapter = _context.Chapters.Include(c => c.Series).Include(c => c.Pages).First(x => x.ChapterId == chapterId);
            BasicChapterData model = new()
            {
                Pages = [.. chapter.Pages],
                Layout = layout,
                Direction = direction,
                FirstPageId = chapter.FirstPage
            };
            return View(model);
        }
        
        [AuthorOnly]
        [HttpGet]
        public async Task<IActionResult> PublishLastChapter(Guid seriesId)
        {
            Series series = _context.Series.Include(s => s.Chapters).First(x => x.SeriesId == seriesId);
            if (!series.Chapters.IsNullOrEmpty() && !series.Chapters!.Last().IsPublished)
            {
                series.Chapters!.Last().IsPublished = true;
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }
        
        [AuthorOnly]
        [HttpGet]
        public async Task<IActionResult> DeleteLastChapter(Guid seriesId)
        {
            Series series = _context.Series.Include(s => s.Chapters).Single(x => x.SeriesId == seriesId);
            if (!series.Chapters.IsNullOrEmpty() && !series.Chapters!.Last().IsPublished)
            {
                if ((series.Chapters!.Last().FolderLink is not null) && Directory.Exists(series.Chapters!.Last().FolderLink))
                {
                    Directory.Delete(series.Chapters!.Last().FolderLink, recursive: true);
                }
                _ = _context.Chapters.Remove(series.Chapters!.Last());
                _ = await _context.SaveChangesAsync();
            }
            return RedirectToRoute(new { controller = "Author", action = "EditSeries", seriesId });
        }
    }
}
