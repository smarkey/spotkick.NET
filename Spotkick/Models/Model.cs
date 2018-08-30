using Microsoft.EntityFrameworkCore;
using Spotkick.Models.Bandsintown;
using Spotkick.Models.Spotify;

namespace Spotkick.Models
{
    public class SpotkickContext : DbContext
    {
        public SpotkickContext(DbContextOptions<SpotkickContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<Track> Tracks { get; set; }
    }
}
