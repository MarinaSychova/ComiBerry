using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Chapter
    {
        [Key]
        public Guid ChapterId { get; set; }

        public required string Title { get; set; }

        public required DateTime Date { get; set; }

        public required string FolderLink { get; set; }

        public bool IsPublished { get; set; }

        public required Series Series { get; set; }

        public required ICollection<Page> Pages { get; set; }

        public Guid FirstPage { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
