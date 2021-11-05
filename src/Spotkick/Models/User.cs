using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Spotkick.Models.Spotify;
using Spotkick.Models.Spotify.User;

namespace Spotkick.Models
{
    public class User
    {
        [Key] public int Id { get; set; }
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