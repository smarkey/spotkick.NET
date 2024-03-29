﻿using System;
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
using Microsoft.Extensions.Options;
using MoreLinq.Extensions;
using Polly;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Spotify;
using Spotkick.Models.Spotify.Artist;
using Spotkick.Models.Spotify.Playlist;
using Spotkick.Models.Spotify.Track;
using Spotkick.Models.Spotify.User;
using Spotkick.Properties;
using Spotkick.Utils;

namespace Spotkick.Services.Spotify
{
    public class SpotifyService : ISpotifyService
    {
        public ILogger _logger { get; set; }
        public HttpClient _client { get; set; }
        public JsonSerializerOptions _serializerOptions { get; set; }
        private IUserService _userService { get; }
        private IArtistService _artistService { get; }
        private readonly SpotifyConfig _spotifyConfig;

        public SpotifyService(ILogger<SpotifyService> logger, IUserService userService, IArtistService artistService,
            IOptions<SpotifyConfig> config)
        {
            _logger = logger;
            _spotifyConfig = config.Value;
            _client = new HttpClient
            {
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
            };
            _userService = userService;
            _artistService = artistService;
        }

        public string AuthorizeUrl()
        {
            return $"{_spotifyConfig.AuthorizeUrl}?" +
                   $"scope={HttpUtility.UrlEncode("user-read-email user-follow-read playlist-modify-private")}&" +
                   $"client_id={_spotifyConfig.ClientId}&" +
                   $"redirect_uri={HttpUtility.UrlEncode(_spotifyConfig.CallbackUrl.ToString())}&" +
                   "response_type=code&" +
                   "state=NA&" +
                   "show_dialog=false";
        }

        private string AuthKey => Convert.ToBase64String(
            Encoding.ASCII.GetBytes($"{_spotifyConfig.ClientId}:{_spotifyConfig.ClientSecret}"));

        private async Task<SpotifyUser> GetSpotifyUser(Token spotifyToken)
        {
            _logger.LogInformation("Retrieving user details from Spotify Service");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(spotifyToken.TokenType, spotifyToken.AccessToken);

            var response = await _client.GetAsync($"{_spotifyConfig.ApiUrl}v1/me");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            return await response.Content.ReadFromJsonAsync<SpotifyUser>(_serializerOptions);
        }

        public async Task<User> AuthorizeUser(string spotifyAuthCode)
        {
            _logger.LogInformation("Authenticating user with Spotify service");

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthKey);

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", spotifyAuthCode },
                { "redirect_uri", _spotifyConfig.CallbackUrl.ToString() }
            };

            var response = await _client.PostAsync($"{_spotifyConfig.AccountsUrl}/token",
                new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var authToken = await response.Content.ReadFromJsonAsync<Token>(_serializerOptions);

            var spotifyUser = await GetSpotifyUser(authToken);

            var user = await _userService.GetUserBySpotifyId(spotifyUser.Id);
            if (user != null) return user;

            user = new User
            {
                SpotifyUserId = spotifyUser.Id,
                UserName = $"{spotifyUser.Id}@spotkick.com",
                Email = $"{spotifyUser.Id}@spotkick.com",
                DisplayName = spotifyUser.DisplayName,
                Token = authToken
            };

            await _userService.CreateUser(user);

            return await _userService.GetUserBySpotifyId(spotifyUser.Id);
        }

        private async Task<User> RefreshAccessTokenIfNecessary(User user)
        {
            if (!user.Token.NeedsRefresh()) return user;

            _logger.LogInformation("Refreshing auth token for {DisplayName}", user.DisplayName);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", AuthKey);

            var requestData = new Dictionary<string, string>
            {
                { "grant_type", "refresh_token" },
                { "refresh_token", user.Token.RefreshToken }
            };

            var response = await _client.PostAsync($"{_spotifyConfig.AccountsUrl}/token",
                new FormUrlEncodedContent(requestData));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var refreshedToken = await response.Content.ReadFromJsonAsync<Token>(_serializerOptions);
            refreshedToken.RefreshToken = user.Token.RefreshToken;

            user.Token = refreshedToken;
            await _userService.UpdateUser(user);

            return user;
        }

        public async Task<IEnumerable<Artist>> GetFollowedArtists(string spotifyUserId)
        {
            var user = await _userService.GetUserBySpotifyId(spotifyUserId);
            _logger.LogInformation("Fetching Spotify artists followed by {DisplayName}", user.DisplayName);

            user = await RefreshAccessTokenIfNecessary(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(user.Token.TokenType, user.Token.AccessToken);

            var artistsFromSpotify = new List<Artist>();
            var url = $"{_spotifyConfig.ApiUrl}v1/me/following?type=artist&limit=50";

            while (true)
            {
                var response = await _client.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

                var responseContent = await response.Content.ReadFromJsonAsync<ArtistResultsPage>(_serializerOptions);

                artistsFromSpotify.AddRange(responseContent?.Artists.Items.Select(a => a.ToSpotkickArtist()) ??
                                            Array.Empty<Artist>());

                var nextUrl = responseContent?.Artists.Next;
                if (nextUrl == null) break;

                url = nextUrl;
            }

            await _artistService.CreateArtists(await _artistService.IdentifyUncachedArtists(artistsFromSpotify));
            var artists = await _artistService.GetArtistsBySpotifyId(artistsFromSpotify.Select(_ => _.SpotifyId));

            return artists;
        }

        public async Task<IEnumerable<Track>> GetMostPopularTracks(IEnumerable<Artist> artists, string spotifyUserId,
            int numberOfTracks = 1)
        {
            if (numberOfTracks is < 1 or > 10)
                throw new ArgumentOutOfRangeException(nameof(numberOfTracks),
                    Resources.SpotifyService_GetMostPopularTracks_range_for_number_of_tracks);

            var user = await _userService.GetUserBySpotifyId(spotifyUserId);
            user = await RefreshAccessTokenIfNecessary(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(user.Token.TokenType, user.Token.AccessToken);

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
                    _client.GetAsync($"{_spotifyConfig.ApiUrl}v1/artists/{artist.SpotifyId}/top-tracks?country=GB"));

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

                var responseContent =
                    await response.Content.ReadFromJsonAsync<TopTracksResultsPage>(_serializerOptions);

                var mostPopularTrackForArtist = responseContent?.Tracks
                    .OrderByDescending(a => a.Popularity)
                    .Take(numberOfTracks) ?? new List<Track>();

                tracks.AddRange(mostPopularTrackForArtist);
            }

            return tracks;
        }

        public async Task<Playlist> CreatePlaylist(IEnumerable<Track> tracks, string spotifyUserId)
        {
            var user = await _userService.GetUserBySpotifyId(spotifyUserId);
            user = await RefreshAccessTokenIfNecessary(user);

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue(user.Token.TokenType, user.Token.AccessToken);

            var playlistName = $"Events after {DateTime.Today:yy-MM-dd} [Spotkick]";
            var playlistJson = $"{{\"name\":\"{playlistName}\",\"public\":false}}";

            var response = await _client.PostAsync($"{_spotifyConfig.ApiUrl}v1/users/{spotifyUserId}/playlists",
                new StringContent(playlistJson, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"{response.StatusCode}: {response.ReasonPhrase}");

            var responseContent = await response.Content.ReadFromJsonAsync<PlaylistResultsPage>(_serializerOptions);

            var playlist = new Playlist
            {
                SpotifyId = responseContent?.Id,
                Name = playlistName,
                OwnedBy = user
            };

            await _artistService.CreatePlaylist(playlist);

            return await AddTracksToPlaylist(tracks, playlist);
        }

        private async Task<Playlist> AddTracksToPlaylist(IEnumerable<Track> tracks, Playlist playlist)
        {
            _logger.LogInformation("Adding {NumberOfTracks} tracks to {PlaylistName}", tracks.Count(), playlist.Name);

            const int batchSize = 100;
            var batches = tracks.Batch(batchSize);

            foreach (var batch in batches)
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
                    _client.PostAsync($"{_spotifyConfig.ApiUrl}v1/playlists/{playlist.SpotifyId}/tracks",
                        new StringContent(tracksPayload, Encoding.UTF8, "application/json")));
            }

            return playlist;
        }
    }
}