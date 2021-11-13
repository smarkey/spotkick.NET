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
using Spotkick.Interfaces.Songkick;
using Spotkick.Models;
using Spotkick.Models.Songkick;
using Spotkick.Models.Songkick.Artist;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Services.Songkick
{
    public class SongkickService : ISongkickService
    {
        public ILogger _logger { get; set; }
        public HttpClient _client { get; set; }
        public JsonSerializerOptions _serializerOptions { get; set; }
        private readonly SongkickConfig _songkickConfig;

        public SongkickService(ILogger<SongkickService> logger, IOptions<SongkickConfig> config)
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
        }

        public async Task<SongkickArtist> GetArtistByName(string artistName)
        {
            var response = await _client.GetAsync(
                "/api/3.0/search/artists.json?" +
                $"apikey={_songkickConfig.Key}&" +
                $"query={HttpUtility.UrlEncode(artistName)}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<ArtistResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Artist?.FirstOrDefault();
        }

        public async Task<IEnumerable<Event>> GetEventsForArtistsByArtistName(string artistName)
        {
            var artist = await GetArtistByName(artistName);

            return await GetEventsForArtistBySongkickArtistId(artist.Id);
        }

        private async Task<IEnumerable<Event>> GetEventsForArtistBySongkickArtistId(int? songkickArtistId)
        {
            var response = await _client.GetAsync(
                $"/api/3.0/artists/{songkickArtistId}/calendar.json?" +
                $"apikey={_songkickConfig.Key}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<EventResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Event ?? new List<Event>();
        }

        public async Task<IEnumerable<Artist>> FilterArtistsWithEventsInLocation(IEnumerable<Artist> artists,
            Location location)
        {
            _logger.LogInformation("Filtering artists on those performing in {City}", location.City);

            return artists.Where(artist =>
            {
                _logger.LogDebug("Checking for {ArtistName} events in {City}", artist.Name, location.City);

                if (artist.SongkickId != null)
                {
                    var spotifyArtistEvents = GetEventsForArtistBySongkickArtistId(artist.SongkickId).Result;
                    var spotifyArtistEventsInLocation = spotifyArtistEvents.Where(e =>
                        e.Status == "ok" &&
                        e.Location.City.Contains(location.City));

                    if (spotifyArtistEventsInLocation.Any()) return true;
                }

                _logger.LogWarning("{ArtistName} has no Songkick ID", artist.Name);
                return false;
            });
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
            .Select(performance => performance.Artist.ToSpotkickArtist())
            .Distinct();
    }
}