using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spotkick.Models;
using Spotkick.Models.Bandsintown;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Spotkick.Services
{
    public class BandsintownService
    {
        private readonly ILogger<BandsintownService> _logger;
        private readonly HttpClient _client;
        public string _serviceUrl = "https://rest.bandsintown.com/";

        public BandsintownService()
        {
            var loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            _logger = loggerFactory.CreateLogger<BandsintownService>();

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public List<Artist> MatchArtists(List<Artist> spotifyArtists)
        {
            var count = spotifyArtists.Count();
            return spotifyArtists.Select((spotifyArtist, i) =>
            {
                _logger.LogInformation($"{i + 1}/{count} - Matching '{spotifyArtist.Name}'");

                var url = $"{_serviceUrl}artists/{ReplaceUnsupportedChars(spotifyArtist.Name)}?app_id={Properties.Resources.BandsintownAppId}";

                var request = _client.GetAsync(url);

                if (request.Result.StatusCode != HttpStatusCode.OK) throw new HttpRequestException($"{request.Result.ReasonPhrase}: {url}");

                var response = request.Result.Content.ReadAsStringAsync().Result;

                if (response == "\"\"") return null;

                var responseData = JObject.Parse(response);

                if (!(responseData["upcoming_event_count"]?.Value<int>() > 0)) return null;

                var bandsintownArtist = JsonConvert.DeserializeObject<Artist>(response, CommonService.SerializerSettings);
                bandsintownArtist.SpotifyId = spotifyArtist.SpotifyId;
                return bandsintownArtist;
            })
            .Where(artist => artist != null)
            .ToList();
        }

        public List<Event> Events(List<Artist> bandsintownArtists, DateRange dateRange)
        {
            var count = bandsintownArtists.Count();
            return bandsintownArtists.SelectMany((bandsintownArtist, i) =>
            {
                _logger.LogInformation($"{i + 1}/{count} - Events for '{bandsintownArtist.Name}'");

                var request = _client.GetAsync($"{_serviceUrl}artists/{ReplaceUnsupportedChars(bandsintownArtist.Name)}/events?" +
                                              $"app_id={Properties.Resources.BandsintownAppId}{dateRange.UrlEncodedString()}");
                var response = request.Result.Content.ReadAsStringAsync().Result;
                var events = JsonConvert.DeserializeObject<IEnumerable<Event>>(response, CommonService.SerializerSettings);

                return events.Where(e => e.Venue.City.Contains("Bristol"));
            })
            .ToList();
        }

        public string ReplaceUnsupportedChars(string text)
        {
            return text
                .Replace("?", "%253F")
                .Replace("/", "%252F")
                .Replace("*", "%252A")
                .Replace("\"", "%27C");
        }
    }
}
