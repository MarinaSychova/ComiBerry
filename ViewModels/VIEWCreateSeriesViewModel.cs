using System.ComponentModel.DataAnnotations;

namespace ComiBerry.ViewModels
{
    public class VIEWCreateSeriesViewModel
    {
        [Required(ErrorMessage = "Name is required.")]
        public string? Title { get; set; }

        [Required]
        public int? Genre { get; set; }

        [Required]
        public string? Language { get; set; }

        [Required]
        public string? AgeRestriction { get; set; }

        [Required]
        public string? Layout { get; set; }

        [Required]
        public string? Direction { get; set; }

        [Required]
        public string? AccessMode { get; set; }

        [Required]
        public int FreeChapters { get; set; }

        [Required]
        public bool IsVisible { get; set; }

        public List<Genre>? Genres { get; set; }
    }
}
