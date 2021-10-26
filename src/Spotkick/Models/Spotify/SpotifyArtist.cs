namespace Spotkick.Models.Spotify
{
    public class SpotifyArtist
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Artist ToSpotkickArtist()
        {
            return new Artist
            {
                SpotifyId = Id,
                Name = Name
            };
        }
    }
}