using System;
using System.Collections.Generic;

namespace Spotkick.Models.Songkick.Event
{
    public class Event
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public Uri Uri { get; set; }
        public string DisplayName { get; set; }
        public object Start { get; set; }
        public object End { get; set; }
        public List<Performance> Performance { get; set; }
        public Location Location { get; set; }
        public object Venue { get; set; }
        public string Status { get; set; }
        public string AgeRestriction { get; set; }
    }
}