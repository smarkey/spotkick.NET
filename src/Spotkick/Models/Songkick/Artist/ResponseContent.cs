using System.Collections.Generic;

namespace Spotkick.Models.Songkick.Artist
{
    public class Root
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