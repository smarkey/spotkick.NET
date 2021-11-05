using System;

namespace Spotkick.Models.Spotify
{
    public class SpotifyConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri CallbackUrl { get; set; }
        public Uri AuthorizeUrl { get; set; }
        public Uri AccountsUrl { get; set; }
        public Uri ApiUrl { get; set; }
    }
}