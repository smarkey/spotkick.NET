using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;

namespace Spotkick.Interfaces
{
    public interface ISpotkickService
    {
        Task<User> GetUser(int userId);
        Task<User> GetUser(string userId);
        Task<IEnumerable<Artist>> GetArtists(IEnumerable<long> artistIds);
        Task<IEnumerable<Artist>> GetArtistsFilteredByCity(int userId, string city);
        Task UpdateArtist(Artist artistInSpotify);
        Task CreateUser(User user);
        Task UpdateUser(User user);
        Task CreateArtists(IEnumerable<Artist> identifyUncachedArtists);
        Task<IEnumerable<Artist>> IdentifyUncachedArtists(IEnumerable<Artist> artists);
        Task CreatePlaylist(Playlist playlist);
    }
}