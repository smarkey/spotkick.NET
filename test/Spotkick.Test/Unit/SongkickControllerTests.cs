using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private static readonly User testUser = new()
        {
            Id = Guid.Empty.ToString()
        };

        private readonly IOptions<SpotifyConfig> stubSpotifyConfig = Options.Create(new SpotifyConfig
        {
            AccountsUrl = new Uri("https://stub.api/api"),
            ApiUrl = new Uri("https://stub.api"),
            AuthorizeUrl = new Uri("https://stub.api/authorize"),
            ClientId = "123",
            ClientSecret = "shhhh",
            CallbackUrl = new Uri("https://stub.callback.api/Spotkick/Callback")
        });

        private readonly List<Artist> testArtistsList = new()
        {
            new Artist
            {
                Id = 1,
                SpotifyId = 1.ToString(),
                SongkickId = 1,
                Name = "Test Artist"
            }
        };

        private static UserManager<User> mockGetUserAsync()
        {
            var userManagerMockWithInteractions =
                new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null,
                    null, null);
            userManagerMockWithInteractions
                .Setup(service =>
                    service.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
            return userManagerMockWithInteractions.Object;
        }

        private static SignInManager<User> mockSignInAsync()
        {
            var userManagerMock =
                new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null,
                    null, null);
            var contextAccessorMock = new Mock<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var signInManagerMockWithInteractions = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                contextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                null, null, null, null);
            signInManagerMockWithInteractions
                .Setup(service => service.SignInAsync(testUser, true, It.IsAny<string>()));
            return signInManagerMockWithInteractions.Object;
        }

        private static ISpotifyService mockAuthenticateUser()
        {
            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();
            spotifyServiceMockWithInteractions
                .Setup(service => service.AuthenticateUser(It.IsAny<string>()))
                .ReturnsAsync(testUser);

            return spotifyServiceMockWithInteractions.Object;
        }

        private static IUserService mockGetUserById()
        {
            var userServiceMockWithInteractions = new Mock<IUserService>();
            userServiceMockWithInteractions
                .Setup(service => service.GetUserById(It.IsAny<string>()))
                .ReturnsAsync(testUser);
            return userServiceMockWithInteractions.Object;
        }

        private IArtistService mockGetFollowedArtistsWithEventsUsingAreaCalendar()
        {
            var artistServiceMockWithInteractions = new Mock<IArtistService>();
            artistServiceMockWithInteractions
                .Setup(service =>
                    service.GetFollowedArtistsWithEventsUsingAreaCalendar(It.IsAny<string>(), It.IsAny<Location>()))
                .ReturnsAsync(testArtistsList);
            return artistServiceMockWithInteractions.Object;
        }

        private IArtistService mockGetArtistsById()
        {
            var artistServiceMockWithInteractions = new Mock<IArtistService>();
            artistServiceMockWithInteractions
                .Setup(service => service.GetArtistsById(It.IsAny<List<long>>()))
                .ReturnsAsync(testArtistsList);
            return artistServiceMockWithInteractions.Object;
        }

        private ISpotifyService mockGetMostPopularTracksAndCreatePlaylist()
        {
            var trackList = new List<Track>
            {
                new()
                {
                    Id = 1.ToString(),
                    Name = "Track Name",
                    Popularity = 1
                }
            };
            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();
            spotifyServiceMockWithInteractions
                .Setup(service => service.GetMostPopularTracks(testArtistsList, It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(trackList);

            spotifyServiceMockWithInteractions
                .Setup(service => service.CreatePlaylist(trackList, It.IsAny<string>()))
                .ReturnsAsync(new Playlist());

            return spotifyServiceMockWithInteractions.Object;
        }

        [Fact]
        public async Task WhenINavigateToIndex_ThenIShouldLandOnTheHomePage()
        {
            // Arrange
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                stubSpotifyConfig,
                It.IsAny<UserManager<User>>(),
                It.IsAny<SignInManager<User>>());

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
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                stubSpotifyConfig,
                It.IsAny<UserManager<User>>(),
                It.IsAny<SignInManager<User>>());

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
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                It.IsAny<ArtistService>(),
                stubSpotifyConfig,
                mockGetUserAsync(),
                mockSignInAsync());

            _sut.SpotifyService = mockAuthenticateUser();

            // Act
            var result = await _sut.Callback("auth_code_string_provided_by_spotify");

            // Assert
            result.ShouldBeOfType<RedirectResult>();
            result.Url.ShouldBe("Discovery");
        }

        [Fact]
        public async Task WhenINavigateToDiscovery_ThenIShouldLandOnTheDiscoveryPage()
        {
            // Arrange
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                mockGetUserById(),
                It.IsAny<ArtistService>(),
                stubSpotifyConfig,
                mockGetUserAsync(),
                It.IsAny<SignInManager<User>>());

            // Act
            var result = await _sut.Discovery() as ViewResult;

            // Assert
            ((User)result?.ViewData["User"])?.ShouldBe(testUser);
        }

        [Fact]
        public async Task WhenINavigateToSelection_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            var city = "New York";
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                mockGetFollowedArtistsWithEventsUsingAreaCalendar(),
                stubSpotifyConfig,
                mockGetUserAsync(),
                It.IsAny<SignInManager<User>>());

            // Act
            var result = await _sut.Selection(city) as ViewResult;

            // Assert
            result?.ViewData["City"]?.ShouldBe(city);
            result?.ViewData["UserId"]?.ShouldBe(testUser.Id);
            result?.ViewData["SpotifyArtists"]?.ShouldBe(testArtistsList);
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }

        [Fact]
        public async Task WhenIHaveSelectedSomeArtistsAndProceedToCreateAPlaylist_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            var _sut = new SpotkickController(
                It.IsAny<ILogger<SpotkickController>>(),
                It.IsAny<UserService>(),
                mockGetArtistsById(),
                stubSpotifyConfig,
                mockGetUserAsync(),
                It.IsAny<SignInManager<User>>());

            _sut.SpotifyService = mockGetMostPopularTracksAndCreatePlaylist();

            // Act
            var result = await _sut.Playlist(new List<long>(), 1) as ViewResult;

            // Assert
            result?.ViewData["Playlist"].ShouldBeOfType<Playlist>();
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }
    }
}