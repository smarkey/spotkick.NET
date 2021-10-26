using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;

namespace Spotkick.Interfaces
{
    public interface ISongkickService : IThirdPartyService
    {
        Task<IEnumerable<Artist>> GetArtistsWithUpcomingEvents(IEnumerable<Artist> followedSpotifyArtists);

        Task<IEnumerable<Artist>> FilterArtistsWithEventsInCity(IEnumerable<Artist> artistsWithUpcomingGigs,
            string city);
    }
}