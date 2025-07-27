namespace ComiBerry.ViewModels
{
    public class SeriesIdViewModel
    {
        public required Guid SeriesId { get; set; }
    }

    public class BasicSeriesData
    {
        public required Guid Id { get; set; }

        public required string Title { get; set; }

        public required int Year { get; set; }
    }

    public class ExtendedSeriesData
    {
        public required BasicSeriesData BasicSeriesDataPart { get; set; }

        public required string Language { get; set; }

        public required string AgeRestriction { get; set; }

        public required string Layout { get; set; }

        public required string Direction { get; set; }

        public string? Description { get; set; }

        public string? Tags { get; set; }

        public int ViewCount { get; set; }

        public required List<Genre> Genres { get; set; }

        public required string UserId { get; set; }

        public required string UserName { get; set; }

        public int? LikeCount { get; set; }

        public required List<Chapter> Chapters { get; set; }
    }

    public class SeriesListViewModel
    {
        public required List<BasicSeriesData> Series { get; set; }
    }

    public class VIEWEditSeriesViewModel
    {
        public required ExtendedSeriesData ExtendedSeriesDataPart { get; set; }

        public required bool IsVisible { get; set; }

        public required List<Genre> Genres { get; set; }
    }

    public class ADMINGetSeriesViewModel
    {
        public required List<BasicSeriesData> Series;

        public string? Id { get; set; }

        public string? Title { get; set; }

        public int? Year { get; set; }
    }
}
