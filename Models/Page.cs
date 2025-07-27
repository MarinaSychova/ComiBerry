using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class Page
    {
        [Key]
        public Guid PageId { get; set; }

        public required string PageLink { get; set; }

        public Page? NextPage { get; set; }

        public required Chapter Chapter { get; set; }
    }
}
