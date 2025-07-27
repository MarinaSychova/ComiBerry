using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ComiBerry.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User>(options)
    {
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Series> Series { get; set; }
        public DbSet<Chapter> Chapters { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Fave> Faves { get; set; }
        public DbSet<Comment> Comments { get; set; }
    }
}
