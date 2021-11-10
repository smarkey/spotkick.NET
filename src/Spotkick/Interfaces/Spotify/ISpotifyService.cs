using System.Collections.Generic;
using System.Threading.Tasks;
using Spotkick.Models;
using Spotkick.Models.Spotify.Track;

namespace Spotkick.Interfaces.Spotify
{
    public interface ISpotifyService : IThirdPartyService
    {
        string AuthenticationUrl();
        Task<User> AuthenticateUser(string spotifyAuthCode);
        Task<IEnumerable<Artist>> GetFollowedArtists(string userId);
        Task<IEnumerable<Track>> GetMostPopularTracks(IEnumerable<Artist> artists, string spotifyUserId,
            int numberOfTracks = 1);
        Task<Playlist> CreatePlaylist(IEnumerable<Track> tracks, string spotifyUserId);
    }
}