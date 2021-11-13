using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Spotkick.Data;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Songkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Services
{
    public class ArtistService : IArtistService
    {
        private readonly ILogger<ArtistService> _logger;
        private readonly SpotkickDbContext _dbContext;
        private readonly ISongkickService _songkickService;

        public ArtistService(
            ILogger<ArtistService> logger, 
            SpotkickDbContext dbContext, 
            ISongkickService songkickService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _songkickService = songkickService;
        }

        public async Task CreateArtists(IEnumerable<Artist> artists)
        {
            if (artists != null && artists.Any())
            {
                _logger.LogInformation("Creating {NumberOfUncachedArtists} Artists", artists.Count());
                await _dbContext.Artists.AddRangeAsync(artists);
                await _dbContext.SaveChangesAsync();
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
                .Select(artistId => _dbContext.Artists.Find(artistId))
                .Where(a => a != null);
        }

        public async Task<IEnumerable<Artist>> GetArtistsBySpotifyId(IEnumerable<string> artistIds)
        {
            _logger.LogInformation("Retrieving {NumberOfArtists} Artists", artistIds.Count());
            return artistIds
                .Select(artistId => _dbContext.Artists.FirstOrDefault(_ => _.SpotifyId.Equals(artistId)))
                .Where(a => a != null);
        }

        public async Task<IEnumerable<Artist>> FilterArtistsWithEventsUsingAreaCalendar(IEnumerable<Artist> followedArtists, Location location)
        {
            var artistsWithEvents = (await _songkickService.GetArtistsWithEventsInLocation(location)).ToList();

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
            var internalArtist = _dbContext.Artists.FirstOrDefault(a => a.SongkickId.Equals(artist.SongkickId));
            if (internalArtist == null) return;

            artist.Id = internalArtist.Id;
            if (internalArtist.Equals(artist)) return;

            _logger.LogDebug("Updating artist: {Artist}", artist);

            _dbContext.Artists.Update(artist);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Artist>> IdentifyUncachedArtists(IEnumerable<Artist> artists)
        {
            _logger.LogInformation("Identifying uncached Artists");
            return artists.Where(externalArtist =>
                _dbContext.Artists.FirstOrDefault(a => a.SpotifyId == externalArtist.SpotifyId) == null);
        }

        public async Task CreatePlaylist(Playlist playlist)
        {
            _logger.LogInformation("Creating a Playlist: {Playlist}", playlist);
            await _dbContext.Playlists.AddAsync(playlist);
            await _dbContext.SaveChangesAsync();
        }
    }
}