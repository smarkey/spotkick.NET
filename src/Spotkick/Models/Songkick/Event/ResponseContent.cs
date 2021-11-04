using System.Collections.Generic;

namespace Spotkick.Models.Songkick.Event
{
    public class Root
    {
        public ResultsPage ResultsPage { get; set; }
    }

    public class ResultsPage
    {
        public Results Results { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int TotalEntries { get; set; }
    }

    public class Results
    {
        public List<Event> Event { get; set; }
    }
}