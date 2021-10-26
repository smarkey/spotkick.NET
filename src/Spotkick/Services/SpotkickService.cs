using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spotkick.Interfaces;
using Spotkick.Models;

namespace Spotkick.Services
{
    public class SpotkickService : ISpotkickService
    {
        private readonly ILogger _logger;
        private readonly SpotkickContext _db;
        private readonly SpotifyService _spotifyService;
        private readonly SongkickService _songkickService;

        public SpotkickService(ILogger logger, SpotkickContext dbContext)
        {
            _logger = logger;
            _db = dbContext;
            _spotifyService = new SpotifyService(logger, dbContext, this);
            _songkickService = new SongkickService(logger, dbContext, this);
        }

        public async Task CreateUser(User user)
        {
            _logger.LogInformation("Creating a user for {DisplayName}", user.DisplayName);
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User> GetUser(int userId)
        {
            _logger.LogInformation("Retrieving user by ID {UserId}", userId);
            return await _db.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUser(string spotifyUserId)
        {
            _logger.LogInformation("Retrieving user by Spotify ID {SpotifyUserId}", spotifyUserId);
            return await _db.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.SpotifyUserId == spotifyUserId);
        }

        public async Task UpdateUser(User user)
        {
            _logger.LogInformation("Updating user with ID {UserId}", user.Id);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }

        public async Task CreateArtists(IEnumerable<Artist> artistsNotInCache)
        {
            if (artistsNotInCache != null && artistsNotInCache.Any())
            {
                _logger.LogInformation("Creating {NumberOfUncachedArtists} Artists", artistsNotInCache.Count());
                await _db.Artists.AddRangeAsync(artistsNotInCache);
                await _db.SaveChangesAsync();
            }
            else
            {
                _logger.LogWarning("Found no uncached Artists");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtists(IEnumerable<long> artistIds)
        {
            _logger.LogInformation("Retrieving {NumberOfArtists} Artists", artistIds.Count());
            var artists = artistIds.Select(artistId => _db.Artists.Find(artistId)).ToList();
            artists.RemoveAll(_ => _.SongkickId == null);
            return artists;
        }

        public async Task<IEnumerable<Artist>> GetArtistsFilteredByCity(int userId, string city)
        {
            var followedArtists = await _spotifyService.GetFollowedArtists(userId);
            var artistsWithUpcomingGigs = await _songkickService.GetArtistsWithUpcomingEvents(followedArtists);
            var artistsFilteredByCity =
                await _songkickService.FilterArtistsWithEventsInCity(artistsWithUpcomingGigs, city);

            return artistsFilteredByCity;
        }

        public async Task UpdateArtist(Artist artist)
        {
            var internalArtist = _db.Artists.FirstOrDefault(a => a.SongkickId.Equals(artist.SongkickId));

            if (internalArtist == null) return;

            artist.Id = internalArtist.Id;

            if (internalArtist.Equals(artist)) return;

            _logger.LogDebug("Updating artist: {Artist}", artist);

            _db.Artists.Update(artist);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Artist>> IdentifyUncachedArtists(IEnumerable<Artist> artists)
        {
            _logger.LogInformation("Identifying uncached Artists");
            var uncachedArtists = new List<Artist>();

            foreach (var externalArtist in artists)
            {
                var internalArtist =
                    await _db.Artists.FirstOrDefaultAsync(a => a.SpotifyId == externalArtist.SpotifyId);

                if (internalArtist == null)
                {
                    uncachedArtists.Add(externalArtist);
                }
            }

            return uncachedArtists;
        }

        public async Task CreatePlaylist(Playlist playlist)
        {
            _logger.LogInformation("Creating a Playlist: {Playlist}", playlist);
            await _db.Playlists.AddAsync(playlist);
            await _db.SaveChangesAsync();
        }
    }
}