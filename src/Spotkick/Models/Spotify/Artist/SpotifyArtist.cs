namespace Spotkick.Models.Spotify.Artist
{
    public class SpotifyArtist
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Models.Artist ToSpotkickArtist()
        {
            return new Models.Artist
            {
                SpotifyId = Id,
                Name = Name
            };
        }
    }
}