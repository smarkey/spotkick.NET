using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Spotkick.Models.Songkick.Event
{
    [DataContract]
    public class EventResultsPage
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