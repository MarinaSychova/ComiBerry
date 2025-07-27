using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [Required]
        public required string Name { get; set; }

        public ICollection<Series>? Series { get; set; }
    }
}
