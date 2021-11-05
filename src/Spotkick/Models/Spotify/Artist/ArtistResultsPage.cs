using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spotkick.Models.Spotify.Artist
{
    [DataContract]
    public class ArtistResultsPage
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