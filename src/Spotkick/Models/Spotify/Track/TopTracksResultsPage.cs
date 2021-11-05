using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spotkick.Models.Spotify.Track
{
    [DataContract]
    public class TopTracksResultsPage
    {
        public IEnumerable<Track> Tracks { get; set; }
    }
}