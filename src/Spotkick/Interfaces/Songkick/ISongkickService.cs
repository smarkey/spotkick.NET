using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Interfaces.Songkick
{
    public interface ISongkickService : IThirdPartyService
    {
        Task<IEnumerable<Artist>> FilterArtistsWithEventsInLocation(IEnumerable<Artist> artists, Location location);
    }
}