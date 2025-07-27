using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Series
    {
        [Key]
        public Guid SeriesId { get; set; }

        public required string Title { get; set; }

        public required string Language { get; set; }

        public required int Year { get; set; }

        public required string AgeRestriction { get; set; }

        public required string Layout { get; set; }

        public required string Direction { get; set; }

        public required string AccessMode { get; set; }

        public required int FreeChapters { get; set; }

        public required bool IsVisible { get; set; }

        public required string FolderLink { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(1000)]
        public string? Tags { get; set; }

        public string? CoverLink { get; set; }

        public int ViewCount { get; set; }

        public User? User { get; set; }

        public required ICollection<Genre> Genre { get; set; }

        public ICollection<Chapter>? Chapters { get; set; }

        public ICollection<Fave>? Fave { get; set; }
    }
}
