using System.ComponentModel.DataAnnotations;

namespace Spotkick.Models.Bandsintown
{
    public class Artist
    {
        [Key]
        public int PrimaryKeyId { get; set; }

        public int Id { get; set; }
        public string SpotifyId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbUrl { get; set; }
        public string FacebookPageUrl { get; set; }
        public string Mbid { get; set; }
        public int TrackerCount { get; set; }
        public int UpcomingEventCount { get; set; }
    }
}
