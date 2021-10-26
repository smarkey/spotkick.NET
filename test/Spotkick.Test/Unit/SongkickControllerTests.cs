using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Spotkick.Controllers;
using Spotkick.Enums;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Models.Spotify;
using Xunit;

namespace Spotkick.Test.Unit
{
    public class SongkickControllerTests
    {
        private readonly SpotkickController _sut;
        private readonly User _testUser;

        public SongkickControllerTests()
        {
            var mockLogger = new Mock<ILogger<SpotkickController>>().Object;
            var mockDbOptions = new Mock<SpotkickContext>(new DbContextOptionsBuilder<SpotkickContext>().Options)
                .Object;
            _sut = new SpotkickController(mockLogger, mockDbOptions);
            _testUser = new User { Id = 999 };
        }

        [Fact]
        public async Task WhenINavigateToTheIndexPage_ThenIAmRedirectedToSpotifyToLogin()
        {
            // Arrange & Act
            var result = _sut.Index();

            // Assert
            result.ShouldBeOfType<RedirectResult>();
            result.Url.ShouldContain("spotify");
        }

        [Fact]
        public async Task WhenSpotifyUsesTheCallback_ThenIAmRedirectedToTheDiscoveryPage()
        {
            // Arrange
            const string spotifyAuthCode = "auth_code_string_provided_by_spotify";

            var spotifyServiceMock = new Mock<ISpotifyService>();
            spotifyServiceMock
                .Setup(service => service.AuthenticateUser(It.IsAny<string>()))
                .ReturnsAsync(_testUser);

            _sut.SpotifyService = spotifyServiceMock.Object;

            // Act
            var result = await _sut.Callback(spotifyAuthCode);

            // Assert
            result.ShouldBeOfType<RedirectResult>();
            result.Url.ShouldBe($"Discovery?userId={_testUser.Id}");
        }

        [Fact]
        public async Task WhenTheDiscoveryUrlIsCalled_ThenIAmServedUpTheDiscoveryPage()
        {
            // Arrange
            var spotkickServiceMock = new Mock<ISpotkickService>();
            spotkickServiceMock
                .Setup(service => service.GetUser(It.IsAny<int>()))
                .ReturnsAsync(_testUser);

            _sut.SpotkickService = spotkickServiceMock.Object;

            // Act
            var result = await _sut.Discovery(_testUser.Id) as ViewResult;

            // Assert
            ((User)result?.ViewData["User"])?.ShouldBe(_testUser);
        }

        [Fact]
        public async Task WhenISelectTheDefaultPlaylistOption_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            const string city = "New York";
            var spotkickServiceMock = new Mock<ISpotkickService>();
            var spotifyServiceMock = new Mock<ISpotifyService>();

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

            spotkickServiceMock
                .Setup(service => service.GetArtistsFilteredByCity(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(testArtistsList);

            spotifyServiceMock
                .Setup(service => service.GetMostPopularTracks(testArtistsList, It.IsAny<int>()))
                .ReturnsAsync(trackList);

            spotifyServiceMock
                .Setup(service => service.CreatePlaylist(trackList, It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Playlist());

            _sut.SpotkickService = spotkickServiceMock.Object;
            _sut.SpotifyService = spotifyServiceMock.Object;

            // Act
            var result = await _sut.Selection(_testUser.Id, city, DiscoveryOption.Default) as ViewResult;

            // Assert
            result?.ViewData["City"]?.ShouldBe(city);
            result?.ViewData["UserId"]?.ShouldBe(_testUser.Id);
            result?.ViewData["SpotifyArtists"]?.ShouldBe(testArtistsList);
            result?.ViewName.ShouldBe("Playlist");
        }

        [Fact]
        public async Task WhenISelectTheWizardPlaylistOption_ThenIShouldLandOnTheSelectionPage()
        {
            // Arrange
            const string city = "New York";
            var spotkickServiceMock = new Mock<ISpotkickService>();

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

            spotkickServiceMock
                .Setup(service => service.GetArtistsFilteredByCity(It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(testArtistsList);

            _sut.SpotkickService = spotkickServiceMock.Object;

            // Act
            var result = await _sut.Selection(_testUser.Id, city, DiscoveryOption.Wizard) as ViewResult;

            // Assert
            result?.ViewData["City"]?.ShouldBe(city);
            result?.ViewData["UserId"]?.ShouldBe(_testUser.Id);
            result?.ViewData["SpotifyArtists"]?.ShouldBe(testArtistsList);
            result?.ViewName.ShouldBe(null); // null because it is the default view for the Selection controller method
        }

        [Fact]
        public async Task WhenIHaveSelectedSomeArtistsInTheWizard_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            var spotkickServiceMock = new Mock<ISpotkickService>();
            var spotifyServiceMock = new Mock<ISpotifyService>();

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

            spotkickServiceMock
                .Setup(service => service.GetArtists(It.IsAny<List<long>>()))
                .ReturnsAsync(testArtistsList);

            spotifyServiceMock
                .Setup(service => service.GetMostPopularTracks(testArtistsList, It.IsAny<int>()))
                .ReturnsAsync(trackList);

            spotifyServiceMock
                .Setup(service => service.CreatePlaylist(trackList, It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new Playlist());

            _sut.SpotkickService = spotkickServiceMock.Object;
            _sut.SpotifyService = spotifyServiceMock.Object;

            // Act
            var result = await _sut.Playlist(_testUser.Id, new List<long>()) as ViewResult;

            // Assert
            result?.ViewData["Playlist"].ShouldBeOfType<Playlist>();
            result?.ViewName.ShouldBe(null); // null because it is the default view for the Playlist controller method
        }
    }
}