using System;

namespace Spotkick.Models.Spotify.User
{
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long ExpiresIn { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public bool NeedsRefresh() => Created.AddSeconds(ExpiresIn) <= DateTime.Now;
    }
}