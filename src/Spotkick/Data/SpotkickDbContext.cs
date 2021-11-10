using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotkick.Models;
using Spotkick.Models.Spotify.User;

namespace Spotkick.Data
{
    public class SpotkickDbContext : IdentityDbContext<User>
    {
        public SpotkickDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Token> Tokens { get; set; }
        public DbSet<Artist> Artists { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
    }
}