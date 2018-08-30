using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spotkick.Models.Spotify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spotkick.Models;
using Spotkick.Models.Bandsintown;

namespace Spotkick.Services
{
    public class SpotifyService
    {
        private readonly ILogger<BandsintownService> _logger;
        private readonly HttpClient _client;
        private readonly SpotkickContext _db;

        public SpotifyService()
        {
            var loggerFactory = new LoggerFactory().AddConsole().AddDebug();
            _logger = loggerFactory.CreateLogger<BandsintownService>();

            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var optionsBuilder = new DbContextOptionsBuilder<SpotkickContext>();
            optionsBuilder.UseSqlServer(Properties.Resources.DbConnectionString);
            _db = new SpotkickContext(optionsBuilder.Options);
        }

        public string AuthenticationUrl()
        {
            return $"{Properties.Resources.SpotifyAuthorizeUrl}?" +
                   $"scope={HttpUtility.UrlEncode(Properties.Resources.SpotifyScope)}&" +
                   $"client_id={Properties.Resources.SpotifyClientId}&" +
                   $"redirect_uri={HttpUtility.UrlEncode(Properties.Resources.SpotifyRedirectUrl)}&" +
                   "response_type=code&" +
                   "state=NA&" +
                   "show_dialog=false";
        }

        public User CreateUser(string spotifyCode)
        {
            var basicAuthKey = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{Properties.Resources.SpotifyClientId}:{Properties.Resources.SpotifyClientSecret}"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", basicAuthKey);

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", spotifyCode },
                { "redirect_uri", Properties.Resources.SpotifyRedirectUrl }
            };

            var request = _client.PostAsync("https://accounts.spotify.com/api/token", new FormUrlEncodedContent(requestData));
            var response = request.Result.Content.ReadAsStringAsync().Result;

            var authToken = JsonConvert.DeserializeObject<Token>(response, CommonService.SerializerSettings);
            var user = new User
            {
                DisplayName = DisplayName(authToken),
                Token = authToken
            };
            _db.Users.Add(user);
            _db.SaveChanges();

            return user;
        }

        public User GetUser(int id)
        {
            return _db.Users.Find(id);
        }

        public List<Artist> Artists(int userId)
        {
            _logger.LogInformation("Fetching Spotify followed artists...");
            var accessToken = _db.Users.Where(u => u.Id.Equals(userId)).Select(u => u.Token.AccessToken).First();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            var artists = new List<Artist>();
            var url = $"https://api.spotify.com/v1/me/following?type=artist&limit=50";
            var page = 1;

            while (true)
            {
                _logger.LogInformation($"Page {page} of upto 50 artists");

                var request = _client.GetAsync(url);
                request.RunSynchronously();
                request.

                if (!request.Result.IsSuccessStatusCode)
                {
                    Thread.Sleep(1000);
                    request = _client.GetAsync(url);
                }

                var response = request.Result.Content.ReadAsStringAsync().Result;
                var responseData = JObject.Parse(response);

                var artistBatch = responseData["artists"]["items"]
                    .Children()
                    .Select(artist => new Artist
                    {
                        SpotifyId = artist["id"].Value<string>(),
                        Name = artist["name"].Value<string>()
                    })
                    .ToList();

                artists.AddRange(artistBatch);

                var next = responseData["artists"]["next"];
                if (next.Type == JTokenType.Null) break;
                url = next.Value<string>();
                page++;
            }

            var artistsNotInCache = artists.Where(a => _db.Artists.Count(u => u.SpotifyId.Equals(a.SpotifyId)) == 0);

            _db.Artists.AddRange(artistsNotInCache);
            _db.SaveChanges();

            return artists;
        }

        public List<Track> TopTracks(List<Artist> artists, Token spotifyToken)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", spotifyToken.AccessToken);

            var count = artists.Count();
            return artists.Select((artist, i) =>
            {
                _logger.LogInformation($"{i + 1}/{count} - Top Tracks for '{artist.Name}'");

                var request = _client.GetAsync($"https://api.spotify.com/v1/artists/{artist.SpotifyId}/top-tracks?country=GB");
                var response = request.Result.Content.ReadAsStringAsync().Result;
                var responseData = JObject.Parse(response);

                var errorCode = responseData["error"]["status"].Value<int>();
                if (errorCode != 200)
                {
                    var errorMessage = responseData["error"]["message"].Value<string>();
                    throw new AccessViolationException($"{errorCode}: {errorMessage}");
                }

                var tracksData = responseData["tracks"].Children();

                return tracksData["id"].Values<string>().Zip(tracksData["name"].Values<string>(),
                    (id, name) => new Track {Id = id, Name = name})
                    .ToList();
            })
            .SelectMany(i => i)
            .ToList();
        }

        public string DisplayName(Token spotifyToken)
        {
            _logger.LogInformation("Fetching User...");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", spotifyToken.AccessToken);

            var request = _client.GetAsync($"https://api.spotify.com/v1/me");
            var response = request.Result.Content.ReadAsStringAsync().Result;
            var responseData = JObject.Parse(response);

            return responseData["display_name"].Value<string>();
        }
    }
}
