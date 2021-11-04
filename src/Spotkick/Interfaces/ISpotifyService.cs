using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Spotify;
using Spotkick.Models.Spotify.Track;

namespace Spotkick.Interfaces
{
    public interface ISpotifyService : IThirdPartyService
    {
        Task<User> AuthenticateUser(string spotifyAuthCode);
        string AuthenticationUrl();
        Task<IEnumerable<Artist>> GetFollowedArtists(int userId);
        Task<IEnumerable<Track>> GetMostPopularTracks(IEnumerable<Artist> artists, int userId, int numberOfTracks = 1);
        Task<Playlist> CreatePlaylist(IEnumerable<Track> tracks, int userId, string namePrefix = "SpotKick");
    }
}