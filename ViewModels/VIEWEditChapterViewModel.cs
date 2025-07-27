using System.ComponentModel.DataAnnotations;

namespace ComiBerry.ViewModels
{
    public class VIEWEditChapterViewModel
    {
        public Guid SeriesId { get; set; }

        public Guid ChapterId { get; set; }

        public string? Title { get; set; }

        public List<Page>? Pages { get; set; }

        public Guid? FirstPageId { get; set; }

        [Required]
        public List<PageData>? PagesData { get; set; }

        public List<IFormFile>? NewPages { get; set; }
    }

    public class PageData
    {
        public Guid? PageId { get; set; }

        public Guid? NextPageId { get; set; }
    }
}
