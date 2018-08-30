namespace Spotkick.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public User OwnedBy { get; set; }
        private string SpotifyPlaylistId { get; set; }
    }
}
