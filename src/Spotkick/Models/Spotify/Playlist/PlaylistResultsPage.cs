using System.Runtime.Serialization;

namespace Spotkick.Models.Spotify.Playlist
{
    [DataContract]
    public class PlaylistResultsPage
    {
        public string Id { get; set; }
    }
}