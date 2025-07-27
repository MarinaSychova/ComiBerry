using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Fave
    {
        [Key]
        public Guid FaveId { get; set; } = Guid.NewGuid();

        public required User User { get; set; }

        public required Series Series { get; set; }
    }
}
