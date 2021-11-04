using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Interfaces
{
    public interface ISongkickService : IThirdPartyService
    {
        Task<IEnumerable<Artist>> GetArtistsWithUpcomingEvents(IEnumerable<Artist> followedSpotifyArtists);

        Task<IEnumerable<Artist>> FilterArtistsWithEventsInLocation(IEnumerable<Artist> artistsWithUpcomingGigs, Location location);
    }
}