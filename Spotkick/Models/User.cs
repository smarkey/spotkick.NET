using System.Collections.Generic;
using Spotkick.Models.Spotify;

namespace Spotkick.Models
{
    public class User
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public Token Token { get; set; }
        public List<Playlist> Playlists { get; set; }
    }
}
