namespace ComiBerry.ViewModels
{
    public class BasicChapterData
    {
        public required List<Page> Pages { get; set; }

        public required string Layout { get; set; }

        public required string Direction { get; set; }

        public required Guid FirstPageId { get; set; }
    }

    public class VIEWViewChapterViewModel
    {
        public required BasicChapterData BasicChapterDataPart { get; set; }

        public required Guid ChapterId { get; set; }

        public int ChapterIndex { get; set; }

        public required string Title { get; set; }

        public string? SeriesTitle { get; set; }

        public required List<Comment>? Comments { get; set; }
    }
}
