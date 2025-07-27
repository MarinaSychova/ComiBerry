using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Comment
    {
        [Key]
        public Guid CommentId { get; set; } = Guid.NewGuid();

        public required DateTime DateTime { get; set; }

        public required string Text { get; set; }

        public required User User { get; set; }

        public required Chapter Chapter { get; set; }
    }
}
