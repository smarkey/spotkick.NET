using System.Collections.Generic;

namespace Spotkick.Models.Spotify.Artist
{
    public class Root
    {
        public Artists Artists { get; set; }
    }

    public class Artists
    {
        public IEnumerable<SpotifyArtist> Items { get; set; }
        public string Next { get; set; }
        public int Total { get; set; }
        public int Limit { get; set; }
    }
}