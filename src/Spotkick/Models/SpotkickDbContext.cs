using Microsoft.EntityFrameworkCore;
using Spotkick.Models.Spotify.User;

namespace Spotkick.Models
{
    public class SpotkickDbContext : DbContext
    {
        public SpotkickDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
}