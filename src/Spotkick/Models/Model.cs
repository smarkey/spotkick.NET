using Microsoft.EntityFrameworkCore;
using Spotkick.Models.Spotify;

namespace Spotkick.Models
{
    public class SpotkickContext : DbContext
    {
        public SpotkickContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
}