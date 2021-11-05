using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spotkick.Models.Songkick.Artist
{
    [DataContract]
    public class ArtistResultsPage
    {
        public ResultsPage ResultsPage { get; set; }
    }

    public class ResultsPage
    {
        public Results Results { get; set; }
    }

    public class Results
    {
        public List<SongkickArtist> Artist { get; set; }
    }
}