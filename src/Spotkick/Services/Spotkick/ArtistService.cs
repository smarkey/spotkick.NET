using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Services.Spotkick
{
    public class ArtistService : IArtistService
    {
        private readonly ILogger<ArtistService> _logger;
        private readonly SpotkickContext _db;
        private readonly SpotifyService _spotifyService;
        private readonly SongkickService _songkickService;

        public ArtistService(
            ILogger<ArtistService> logger, 
            SpotkickContext dbContext, 
            IUserService userService, 
            IOptions<SpotifyConfig> spotifyConfig, 
            IOptions<SongkickConfig> songkickConfig)
        {
            _logger = logger;
            _db = dbContext;
            _spotifyService = new SpotifyService(logger, userService, this, spotifyConfig);
            _songkickService = new SongkickService(logger, this, songkickConfig);
        }

        public async Task CreateArtists(IEnumerable<Artist> artists)
        {
            if (artists != null && artists.Any())
            {
                _logger.LogInformation("Creating {NumberOfUncachedArtists} Artists", artists.Count());
                await _db.Artists.AddRangeAsync(artists);
                await _db.SaveChangesAsync();
            }
            else
            {
                _logger.LogInformation("Found no uncached Artists");
            }
        }

        public async Task<IEnumerable<Artist>> GetArtistsById(IEnumerable<long> artistIds)
        {
            _logger.LogInformation("Retrieving {NumberOfArtists} Artists", artistIds.Count());
            return artistIds
                .Select(artistId => _db.Artists.Find(artistId))
                .Where(a => a != null);
        }

        public async Task<IEnumerable<Artist>> GetArtistsBySpotifyId(IEnumerable<string> artistIds)
        {
            _logger.LogInformation("Retrieving {NumberOfArtists} Artists", artistIds.Count());
            return artistIds
                .Select(artistId => _db.Artists.FirstOrDefault(_ => _.SpotifyId.Equals(artistId)))
                .Where(a => a != null);
        }

        public async Task<IEnumerable<Artist>> GetArtistsWithUpcomingEventsUsingArtistCalendar(int userId,
            Location location)
        {
            var followedArtists = await _spotifyService.GetFollowedArtists(userId);
            var artistsWithUpcomingGigs = await _songkickService.GetArtistsWithUpcomingEvents(followedArtists);

            return await _songkickService.FilterArtistsWithEventsInLocation(artistsWithUpcomingGigs, location);
        }

        public async Task<IEnumerable<Artist>> GetFollowedArtistsWithEventsUsingAreaCalendar(int userId,
            Location location)
        {
            var artistsWithEvents = (await _songkickService.GetArtistsWithEventsInLocation(location)).ToList();
            var followedArtists = (await _spotifyService.GetFollowedArtists(userId)).ToList();

            return followedArtists
                .Where(followedArtist => artistsWithEvents.Any(_ => _.Name == followedArtist.Name))
                .Select(followedArtist =>
                {
                    followedArtist.SongkickId = artistsWithEvents.First(_ => _.Name == followedArtist.Name).SongkickId;
                    return followedArtist;
                })
                .ToList();
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
            return artists.Where(externalArtist =>
                _db.Artists.FirstOrDefault(a => a.SpotifyId == externalArtist.SpotifyId) == null);
        }

        public async Task CreatePlaylist(Playlist playlist)
        {
            _logger.LogInformation("Creating a Playlist: {Playlist}", playlist);
            await _db.Playlists.AddAsync(playlist);
            await _db.SaveChangesAsync();
        }
    }
}