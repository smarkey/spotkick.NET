using System.Collections.Generic;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Models
{
    public class SelectionViewModel
    {
        public IEnumerable<Artist> Artists { get; init; }
        public User User { get; init; }
        public Location Location { get; init; }
    }
}