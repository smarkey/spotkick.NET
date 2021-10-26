using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Models.Songkick;
using Spotkick.Models.Songkick.Event;
using Spotkick.Properties;
using EventResultsPage = Spotkick.Models.Songkick.Event.Root;
using ArtistResultsPage = Spotkick.Models.Songkick.Artist.Root;

namespace Spotkick.Services
{
    public class SongkickService : ISongkickService
    {
        public ILogger _logger { get; set; }
        public HttpClient _client { get; set; }
        public JsonSerializerOptions _serializerOptions { get; set; }
        private ISpotkickService _spotkickService { get; }

        public SongkickService(ILogger logger, SpotkickContext dbContext, ISpotkickService spotkickService)
        {
            _logger = logger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(Resources.SongkickApiUrl),
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            _spotkickService = spotkickService;
        }

        public async Task<SongkickArtist> GetArtist(string artistName)
        {
            var response = await _client.GetAsync(
                $"api/3.0/search/artists.json?" +
                $"apikey={Resources.SongkickApiKey}&" +
                $"query={HttpUtility.UrlEncode(artistName)}");

            var responseContent = await response.Content.ReadFromJsonAsync<ArtistResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Artist?.First();
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
                _spotkickService.UpdateArtist(artistInSpotify).Wait();

                return artistInSongkick.OnTourUntil != null;
            }).ToList();
                
            artistsWithUpcomingEvents.RemoveAll(_ => _.SongkickId == null);

            return artistsWithUpcomingEvents;
        }

        public async Task<List<Event>> GetEventsForArtist(string artistName)
        {
            var artist = await GetArtist(artistName);

            return await GetEventsForArtist(artist.Id);
        }

        private async Task<List<Event>> GetEventsForArtist(int? songkickArtistId)
        {
            var response =
                await _client.GetAsync(
                    $"api/3.0/artists/{songkickArtistId}/calendar.json?" +
                    $"apikey={Resources.SongkickApiKey}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<EventResultsPage>(_serializerOptions);

            return responseContent?.ResultsPage.Results.Event ?? new List<Event>();
        }

        public async Task<IEnumerable<Artist>> FilterArtistsWithEventsInCity(
            IEnumerable<Artist> artistsWithUpcomingGigs, string city)
        {
            _logger.LogInformation("Filtering artists on those performing in {City}", city);

            var artistsWithUpcomingGigsInLocation = new List<Artist>();

            foreach (var artist in artistsWithUpcomingGigs)
            {
                _logger.LogDebug("Checking for {ArtistName} events in {City}", artist.Name, city);

                if (artist.SongkickId != null)
                {
                    var spotifyArtistEvents = await GetEventsForArtist(artist.SongkickId);
                    var spotifyArtistEventsInCity = spotifyArtistEvents.Where(e =>
                        e.Status == "ok" &&
                        e.Location.City.Contains(city) &&
                        e.Location.City.EndsWith("UK"));

                    if (spotifyArtistEventsInCity.Any())
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

        // public async Task<IEnumerable<Event>> GetEventsForLocation(string cityName, string countryName)
        // {
        //     var searchQuery = $"{cityName}, {countryName}";
        //
        //     var response = await Client.GetAsync(
        //         $"/api/3.0/search/locations.json?query={HttpUtility.UrlEncode(searchQuery)}&apikey={Resources.SongkickApiKey}");
        //
        //     if (!response.IsSuccessStatusCode)
        //         throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");
        //
        //     var responseContent = await response.Content.ReadAsStringAsync();
        //
        //     var metroAreaId =
        //         JObject.Parse(responseContent)["resultsPage"]["results"]["location"][0]["metroArea"]["id"];
        //
        //     // TODO: Paginate
        //     var eventsResponse =
        //         await Client.GetAsync(
        //             $"/api/3.0/metro_areas/{metroAreaId}/calendar.json?apikey={Resources.SongkickApiKey}");
        //
        //     var eventsResponseContent = await response.Content.ReadAsStringAsync();
        //
        //     var eventsBlob = JObject.Parse(responseContent)["resultsPage"]["results"]["event"];
        //
        //     if (eventsBlob == null) return new List<Event>();
        //
        //     var events = JsonConvert.DeserializeObject<List<Event>>(eventsBlob.ToString(), Serializer);
        //
        //     return events;
        // }

        // public async Task<IEnumerable<Artist>> GetArtistsWithEventsInLocation(string cityName, string countryName)
        // {
        //     var events = await GetEventsForLocation(cityName, countryName);
        //
        //     return new NotImplementedException();
        // }
    }
}