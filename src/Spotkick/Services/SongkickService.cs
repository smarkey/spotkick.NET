using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Artist;
using Spotkick.Models.Songkick.Event;
using EventResultsPage = Spotkick.Models.Songkick.Event.Root;
using ArtistResultsPage = Spotkick.Models.Songkick.Artist.Root;

namespace Spotkick.Services
{
    public class SongkickService : ISongkickService
    {
        public ILogger _logger { get; set; }
        public HttpClient _client { get; set; }
        public JsonSerializerOptions _serializerOptions { get; set; }
        private IArtistService _artistService { get; }
        private readonly SongkickConfig _songkickConfig;

        public SongkickService(ILogger logger, IArtistService artistService, IOptions<SongkickConfig> config)
        {
            _logger = logger;
            _songkickConfig = config.Value;
            _client = new HttpClient
            {
                BaseAddress = _songkickConfig.Url,
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _artistService = artistService;
        }

        public async Task<SongkickArtist> GetArtist(string artistName)
        {
            var response = await _client.GetAsync(
                "/api/3.0/search/artists.json?" +
                $"apikey={_songkickConfig.Key}&" +
                $"query={HttpUtility.UrlEncode(artistName)}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<ArtistResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Artist.FirstOrDefault();
        }

        public async Task<IEnumerable<Artist>> GetArtistsWithUpcomingEvents(IEnumerable<Artist> followedSpotifyArtists)
        {
            var artistsWithUpcomingEvents = followedSpotifyArtists.Where(artistInSpotify =>
            {
                _logger.LogDebug("Retrieving events for {ArtistName} from Songkick", artistInSpotify.Name);
                var artistInSongkick = GetArtist(artistInSpotify.Name).Result;

                if (artistInSongkick == null)
                {
                    _logger.LogWarning("Unable to find {ArtistName} in Songkick", artistInSpotify.Name);
                    return false;
                }

                artistInSpotify.SongkickId = artistInSongkick.Id;
                _artistService.UpdateArtist(artistInSpotify).Wait();

                return true;
            }).ToList();

            artistsWithUpcomingEvents.RemoveAll(_ => _.SongkickId == null || _.SpotifyId == null);

            return artistsWithUpcomingEvents;
        }

        public async Task<IEnumerable<Event>> GetEventsForArtist(string artistName)
        {
            var artist = await GetArtist(artistName);

            return await GetEventsForArtist(artist.Id);
        }

        private async Task<IEnumerable<Event>> GetEventsForArtist(int? songkickArtistId)
        {
            var response = await _client.GetAsync(
                $"/api/3.0/artists/{songkickArtistId}/calendar.json?" +
                $"apikey={_songkickConfig.Key}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<EventResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Event ?? new List<Event>();
        }

        public async Task<IEnumerable<Artist>> FilterArtistsWithEventsInLocation(
            IEnumerable<Artist> artistsWithUpcomingGigs, Location location)
        {
            _logger.LogInformation("Filtering artists on those performing in {City}", location.City);

            var artistsWithUpcomingGigsInLocation = new List<Artist>();

            foreach (var artist in artistsWithUpcomingGigs)
            {
                _logger.LogDebug("Checking for {ArtistName} events in {City}", artist.Name, location.City);

                if (artist.SongkickId != null)
                {
                    var spotifyArtistEvents = await GetEventsForArtist(artist.SongkickId);
                    var spotifyArtistEventsInLocation = spotifyArtistEvents.Where(e =>
                        e.Status == "ok" &&
                        e.Location.City.Contains(location.City));

                    if (spotifyArtistEventsInLocation.Any())
                    {
                        artistsWithUpcomingGigsInLocation.Add(artist);
                    }
                }
                else
                {
                    _logger.LogWarning("{ArtistName} has no Songkick ID", artist.Name);
                }
            }

            return artistsWithUpcomingGigsInLocation;
        }

        public async Task<int> GetMetroAreaId(Location location)
        {
            var response = await _client.GetAsync(
                "/api/3.0/search/locations.json?" +
                $"apikey={_songkickConfig.Key}&" +
                $"query={HttpUtility.UrlEncode(location.City)}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();

            return (int)JObject.Parse(responseContent)["resultsPage"]["results"]["location"][0]["metroArea"]["id"];
        }

        public async Task<IEnumerable<Event>> GetEventsForLocation(Location location)
        {
            var metroAreaId = await GetMetroAreaId(location);
            var pageNumber = 1;
            var paginatedEventResult = new List<Event>();

            while (true)
            {
                var response = await _client.GetAsync(
                    $"/api/3.0/metro_areas/{metroAreaId}/calendar.json?" +
                    $"apikey={_songkickConfig.Key}&" +
                    $"page={pageNumber}");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadFromJsonAsync<EventResultsPage>(_serializerOptions);
                var resultsPage = responseContent?.ResultsPage;

                var events = resultsPage?.Results.Event ?? new List<Event>();
                paginatedEventResult.AddRange(events);

                if (resultsPage?.Page * resultsPage?.PerPage > resultsPage?.TotalEntries) break;

                pageNumber++;
            }

            return paginatedEventResult;
        }

        public async Task<IEnumerable<Artist>> GetArtistsWithEventsInLocation(Location location) =>
            (await GetEventsForLocation(location))
            .SelectMany(e => e.Performance)
            .Select(_ => _.Artist.ToSpotkickArtist())
            .Distinct();
    }
}