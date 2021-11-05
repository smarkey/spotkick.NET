using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Spotkick.Controllers;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Models.Spotify;
using Spotkick.Models.Spotify.Track;
using Spotkick.Services;
using Xunit;

namespace Spotkick.Test.Unit
{
    public class SongkickControllerTests
    {
        private SpotkickController _sut;
        private readonly User _testUser = new() { Id = 999 };
        private readonly IOptions<SpotifyConfig> _spotifyConfig = Options.Create(new SpotifyConfig
        {
            AccountsUrl = new Uri("https://stub.api/api"),
            ApiUrl = new Uri("https://stub.api"),
            AuthorizeUrl = new Uri("https://stub.api/authorize"),
            ClientId = "123",
            ClientSecret = "shhhh",
            CallbackUrl = new Uri("https://stub.callback.api/Spotkick/Callback")
        });

        [Fact]
        public async Task WhenINavigateToIndex_ThenIShouldLandOnTheHomePage()
        {
            // Arrange
            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                _spotifyConfig);

            // Act
            var result = _sut.Index() as ViewResult;

            // Assert
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }

        [Fact]
        public async Task WhenINavigateToTheSSOPage_ThenIShouldBeRedirectedToSpotifyToLogin()
        {
            // Arrange
            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                _spotifyConfig);

            // Act
            var result = _sut.Sso();

            // Assert
            result?.ShouldBeOfType<RedirectResult>();
            result?.Url.ShouldContain("/authorize?scope=");
        }

        [Fact]
        public async Task
            WhenIHaveCompletedSpotifyLoginAndGrantedPermissions_ThenIShouldBeRedirectedToTheDiscoveryPage()
        {
            // Arrange
            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                _spotifyConfig);

            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();
            spotifyServiceMockWithInteractions
                .Setup(service => service.AuthenticateUser(It.IsAny<string>()))
                .ReturnsAsync(_testUser);

            _sut.SpotifyService = spotifyServiceMockWithInteractions.Object;

            // Act
            var result = await _sut.Callback("auth_code_string_provided_by_spotify");

            // Assert
            result.ShouldBeOfType<RedirectResult>();
            result.Url.ShouldBe($"Discovery?userId={_testUser.Id}");
        }

        [Fact]
        public async Task WhenINavigateToDiscovery_ThenIShouldLandOnTheDiscoveryPage()
        {
            // Arrange
            var userServiceMockWithInteractions = new Mock<IUserService>();
            userServiceMockWithInteractions
                .Setup(service => service.GetUser(It.IsAny<int>()))
                .ReturnsAsync(_testUser);

            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                userServiceMockWithInteractions.Object,
                It.IsAny<ArtistService>(),
                _spotifyConfig);

            // Act
            var result = await _sut.Discovery(_testUser.Id) as ViewResult;

            // Assert
            ((User)result?.ViewData["User"])?.ShouldBe(_testUser);
        }

        [Fact]
        public async Task WhenINavigateToSelection_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange

            const string city = "New York";

            var testArtistsList = new List<Artist>
            {
                new()
                {
                    Id = 1,
                    SpotifyId = 1.ToString(),
                    SongkickId = 1,
                    Name = "Test Artist"
                }
            };

            var artistServiceMockWithInteractions = new Mock<IArtistService>();
            artistServiceMockWithInteractions
                .Setup(service =>
                    service.GetFollowedArtistsWithEventsUsingAreaCalendar(It.IsAny<int>(), It.IsAny<Location>()))
                .ReturnsAsync(testArtistsList);

            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                artistServiceMockWithInteractions.Object,
                _spotifyConfig);

            // Act
            var result = await _sut.Selection(_testUser.Id, city) as ViewResult;

            // Assert
            result?.ViewData["City"]?.ShouldBe(city);
            result?.ViewData["UserId"]?.ShouldBe(_testUser.Id);
            result?.ViewData["SpotifyArtists"]?.ShouldBe(testArtistsList);
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }

        [Fact]
        public async Task WhenIHaveSelectedSomeArtistsAndProceedToCreateAPlaylist_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            var testArtistsList = new List<Artist>
            {
                new()
                {
                    Id = 1,
                    SpotifyId = 1.ToString(),
                    SongkickId = 1,
                    Name = "Test Artist"
                }
            };

            var trackList = new List<Track>
            {
                new()
                {
                    Id = 1.ToString(),
                    Name = "Track Name",
                    Popularity = 1
                }
            };

            var artistServiceMockWithInteractions = new Mock<IArtistService>();
            artistServiceMockWithInteractions
                .Setup(service => service.GetArtistsById(It.IsAny<List<long>>()))
                .ReturnsAsync(testArtistsList);

            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();
            spotifyServiceMockWithInteractions
                .Setup(service => service.GetMostPopularTracks(testArtistsList, It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(trackList);

            spotifyServiceMockWithInteractions
                .Setup(service => service.CreatePlaylist(trackList, It.IsAny<int>()))
                .ReturnsAsync(new Playlist());

            _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                artistServiceMockWithInteractions.Object,
                _spotifyConfig);

            _sut.SpotifyService = spotifyServiceMockWithInteractions.Object;

            // Act
            var result = await _sut.Playlist(_testUser.Id, new List<long>(), 1) as ViewResult;

            // Assert
            result?.ViewData["Playlist"].ShouldBeOfType<Playlist>();
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }
    }
}