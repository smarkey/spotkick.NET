using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Spotkick.Models.Spotify.User;

namespace Spotkick.Models
{
    public class User : IdentityUser
    {
        public string DisplayName { get; set; }
        public Token Token { get; set; }
        public List<Playlist> Playlists { get; set; }
        public string SpotifyUserId { get; set; }

        public override string ToString()
        {
            return $"{DisplayName}, ID: {Id}, Spotify: {SpotifyUserId}";
        }
    }
}