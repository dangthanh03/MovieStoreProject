
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieStoreMvc.Models.DTO;

namespace MovieStoreMvc.Models.Domain
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
          
        public DbSet<Movie> Movie { get; set;}
        public DbSet<Genre> Genre { get; set;}
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<MovieGenre> MovieGenre { get; set;}
    }
}
