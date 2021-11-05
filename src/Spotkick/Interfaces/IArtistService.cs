using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Interfaces
{
    public interface IArtistService
    {
        Task CreateArtists(IEnumerable<Artist> artists);
        Task<IEnumerable<Artist>> GetArtistsById(IEnumerable<long> artistIds);
        Task<IEnumerable<Artist>> GetArtistsBySpotifyId(IEnumerable<string> artistIds);
        Task UpdateArtist(Artist artist);
        Task<IEnumerable<Artist>> IdentifyUncachedArtists(IEnumerable<Artist> artists);
        Task CreatePlaylist(Playlist playlist);
        Task<IEnumerable<Artist>> GetFollowedArtistsWithEventsUsingAreaCalendar(int userId, Location location);
    }
}