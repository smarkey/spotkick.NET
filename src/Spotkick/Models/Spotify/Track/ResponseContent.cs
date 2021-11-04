using System.Collections.Generic;

namespace Spotkick.Models.Spotify.Track
{
    public class Root
    {
        public IEnumerable<Track> Tracks { get; set; }
    }
}