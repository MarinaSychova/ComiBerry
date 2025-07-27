using System.ComponentModel.DataAnnotations;

namespace ComiBerry.Models
{
    public class User : IdentityUser
    {
        [Required]
        public override required string UserName { get => base.UserName; set => base.UserName = value; }

        [Required]
        public override string? NormalizedUserName { get => base.NormalizedUserName; set => base.NormalizedUserName = value; }

        [Required]
        public override required string Email { get => base.Email; set => base.Email = value; }

        [Required]
        public override string? NormalizedEmail { get => base.NormalizedEmail; set => base.NormalizedEmail = value; }

        [Required]
        public override string? PasswordHash { get => base.PasswordHash; set => base.PasswordHash = value; }

        public string? AvatarLink { get; set; }

        public string? Bio { get; set; }

        public ICollection<Series>? Series { get; set; }

        public ICollection<Fave>? Fave { get; set; }

        public ICollection<Comment>? Comments { get; set; }
    }
}
