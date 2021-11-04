using System;

namespace Spotkick.Models
{
    public class SpotifyConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public Uri CallbackUrl { get; set; }
        public string AuthorizeUrl { get; set; }
        public Uri AccountsUrl { get; set; }
        public string ApiUrl { get; set; }
    }
}