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
    }

    public class Results
    {
        public List<Event> Event { get; set; }
    }
}