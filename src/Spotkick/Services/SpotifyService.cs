using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Logging;
using MoreLinq.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Models.Spotify;
using Spotkick.Properties;
using Spotkick.Utils;

namespace Spotkick.Services
{
    public class SpotifyService : ISpotifyService
    {
        public ILogger _logger { get; set; }
        public HttpClient _client { get; set; }
        public JsonSerializerOptions _serializerOptions { get; set; }
        private ISpotkickService _spotkickService { get; }
        
        public SpotifyService(ILogger logger, SpotkickContext dbContext, ISpotkickService spotkickService)
        {
            _logger = logger;
            _client = new HttpClient
            {
                BaseAddress = new Uri(Resources.SpotifyApiUrl),
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new SnakeCaseNamingPolicy(),
            };
            _spotkickService = spotkickService;
        }

        public string AuthenticationUrl() { 
                return $"{Resources.SpotifyAuthorizeUrl}?" +
            $"scope={HttpUtility.UrlEncode(Resources.SpotifyScope)}&" +
            $"client_id={Resources.SpotifyClientId}&" +
            $"redirect_uri={HttpUtility.UrlEncode(Resources.SpotifyRedirectUrl)}&" +
            "response_type=code&" +
            "state=NA&" +
            "show_dialog=false";
        }
        
        private static string AuthKey => Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{Resources.SpotifyClientId}:{Resources.SpotifyClientSecret}"));

        private async Task<SpotifyUser> GetSpotifyUser(Token spotifyToken)
        {
            _logger.LogInformation("Retrieving user details from Spotify Service");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", spotifyToken.AccessToken);

            var response = await _client.GetAsync("/v1/me");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            return await response.Content.ReadFromJsonAsync<SpotifyUser>(_serializerOptions);
        }

        public async Task<User> AuthenticateUser(string spotifyAuthCode)
        {
            _logger.LogInformation("Authenticating user with Spotify service");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthKey);

            var requestData = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", spotifyAuthCode},
                {"redirect_uri", Resources.SpotifyRedirectUrl}
            };

            var response = await _client.PostAsync($"{Resources.SpotifyAccountsApiUrl}/token",
                new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var authToken = await response.Content.ReadFromJsonAsync<Token>(_serializerOptions);

            var spotifyUser = await GetSpotifyUser(authToken);

            var user = await _spotkickService.GetUser(spotifyUser.Id);
            if (user != null) return user;

            user = new User
            {
                SpotifyUserId = spotifyUser.Id,
                DisplayName = spotifyUser.DisplayName,
                Token = authToken
            };

            await _spotkickService.CreateUser(user);

            return user;
        }

        private async Task<User> RefreshAccessToken(User user)
        {
            _logger.LogInformation("Refreshing auth token for {DisplayName}", user.DisplayName);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthKey);

            var requestData = new Dictionary<string, string>
            {
                {"grant_type", "refresh_token"},
                {"refresh_token", user.Token.RefreshToken}
            };

            var response = await _client.PostAsync($"{Resources.SpotifyAccountsApiUrl}/token",
                new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var authToken = await response.Content.ReadFromJsonAsync<Token>(_serializerOptions);

            user.Token.AccessToken = authToken?.AccessToken;

            await _spotkickService.UpdateUser(user);

            return user;
        }

        public async Task<IEnumerable<Artist>> GetFollowedArtists(int userId)
        {
            var user = await _spotkickService.GetUser(userId);
            _logger.LogInformation("Fetching Spotify artists followed by {DisplayName}", user.DisplayName);

            user = await RefreshAccessToken(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", user.Token.AccessToken);

            var artists = new List<Artist>();
            var url = "/v1/me/following?type=artist&limit=50";

            while (true)
            {
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JObject.Parse(responseContent);

                var spotifyArtists =
                    JsonConvert.DeserializeObject<List<SpotifyArtist>>(responseData["artists"]["items"].ToString());

                artists.AddRange(spotifyArtists.Select(a => a.ToSpotkickArtist()));

                var next = responseData["artists"]["next"];
                if (next.Type == JTokenType.Null) break;
                url = next.Value<string>();
            }

            await _spotkickService.CreateArtists(await _spotkickService.IdentifyUncachedArtists(artists));

            // await SpotkickService.AddFollowedArtistsToUser();

            return artists;
        }

        public async Task<IEnumerable<Track>> GetMostPopularTracks(IEnumerable<Artist> artists, int userId)
        {
            var user = await _spotkickService.GetUser(userId);
            user = await RefreshAccessToken(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", user.Token.AccessToken);

            var tracks = new List<Track>();

            foreach (var artist in artists)
            {
                var policy = Policy
                    .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(new[]
                        {
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(10)
                        },
                        (exception, timeSpan) =>
                        {
                            _logger.LogWarning(
                                "Encountered {StatusCode}:'{Message}'. Waiting {Period} to retry: Top tracks for {ArtistName}",
                                exception.Result.StatusCode, exception.Result.ReasonPhrase, timeSpan, artist.Name);
                        });

                var response = await policy.ExecuteAsync(() =>
                    _client.GetAsync($"/v1/artists/{artist.SpotifyId}/top-tracks?country=GB"));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseData = JObject.Parse(responseContent);

                var topTenTracksForArtist =
                    JsonConvert.DeserializeObject<List<Track>>(responseData["tracks"].ToString());
                var mostPopularTrackForArtist = topTenTracksForArtist.OrderByDescending(a => a.Popularity).First();
                tracks.Add(mostPopularTrackForArtist);
            }

            return tracks;
        }

        public async Task<Playlist> CreatePlaylist(IEnumerable<Track> tracks, int userId, string namePrefix = "SpotKick")
        {
            var user = await _spotkickService.GetUser(userId);
            user = await RefreshAccessToken(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", user.Token.AccessToken);

            var spotifyUserId = user.SpotifyUserId;
            var playlistName = $"{namePrefix} {Guid.NewGuid().ToString()}";
            var playlistJson = $"{{\"name\":\"{playlistName}\",\"public\":false}}";

            var response = await _client.PostAsync($"/v1/users/{spotifyUserId}/playlists",
                new StringContent(playlistJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadAsStringAsync();

            var playlist = new Playlist
            {
                SpotifyId = JObject.Parse(responseContent)["id"].Value<string>(),
                Name = playlistName,
                OwnedBy = user
            };

            await _spotkickService.CreatePlaylist(playlist);

            return await AddTracksToPlaylist(tracks, playlist);
        }

        private async Task<Playlist> AddTracksToPlaylist(IEnumerable<Track> tracks, Playlist playlist)
        {
            _logger.LogInformation("Adding {NumberOfTracks} tracks to {PlaylistName}", tracks.Count(), playlist.Name);
            
            const int batchSize = 100;
            var batches = tracks.Batch(batchSize);
            
            foreach(var batch in batches)
            {
                var uris = string.Join(",", batch.Select(t => $"\"spotify:track:{t.Id}\""));
                var tracksPayload = $"{{\"uris\":[{uris}]}}";

                var policy = Policy
                    .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.TooManyRequests)
                    .WaitAndRetryAsync(new[]
                        {
                            TimeSpan.FromSeconds(1),
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(10)
                        },
                        (exception, timeSpan) =>
                        {
                            _logger.LogWarning(
                                "Encountered {StatusCode}:'{Message}'. Waiting {Period} to retry: Add tracks to playlist {PlaylistName}",
                                exception.Result.StatusCode, exception.Result.ReasonPhrase, timeSpan, playlist.Name);
                        });

                await policy.ExecuteAsync(() =>
                    _client.PostAsync($"/v1/playlists/{playlist.SpotifyId}/tracks",
                        new StringContent(tracksPayload, Encoding.UTF8, "application/json")));
            }

            return playlist;
        }
    }
}