using System.ComponentModel.DataAnnotations;

namespace Spotkick.Models
{
    public class Playlist
    {
        [Key]
        public int Id { get; set; }
        public string SpotifyId { get; set; }
        public string Name { get; set; }
        public User OwnedBy { get; set; }

        public override string ToString()
        {
            return $"{Name}, ID: {Id}, Spotify: {SpotifyId}, Owner: {OwnedBy.DisplayName}";
        }
    }
}