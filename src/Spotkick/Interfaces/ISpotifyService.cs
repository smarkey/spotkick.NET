using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Spotify;

namespace Spotkick.Interfaces
{
    public interface ISpotifyService : IThirdPartyService
    {
        Task<User> AuthenticateUser(string spotifyAuthCode);
        string AuthenticationUrl();
        Task<IEnumerable<Artist>> GetFollowedArtists(int userId);
        Task<IEnumerable<Track>> GetMostPopularTracks(IEnumerable<Artist> artistsFilteredByCity, int userId);
        Task<Playlist> CreatePlaylist(IEnumerable<Track> tracks, int userId, string namePrefix = "SpotKick");
    }
}