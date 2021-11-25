using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Spotkick.Controllers;
using Spotkick.Models;
using Spotkick.Services;
using Spotkick.Services.Spotify;
using Xunit;
using static Spotkick.Test.Unit.Mocks.Mocks;
using static Spotkick.Test.Unit.TestData.TestData;

namespace Spotkick.Test.Unit
{
    public class SpotkickControllerTests
    {
        [Fact]
        public async Task WhenINavigateToIndex_ThenIShouldLandOnTheHomePage()
        {
            // Arrange
            var _sut = new SpotkickController(
                It.IsAny<SpotifyService>(),
                It.IsAny<ArtistService>(),
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
                mockAuthorizeUrlInSpotifyService(),
                It.IsAny<ArtistService>(),
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
                mockAuthorizeUserInSpotifyService(),
                It.IsAny<ArtistService>(),
                mockGetUserAsyncInUserManager(),
                mockSignInAsyncInSignInManager());

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
                It.IsAny<SpotifyService>(),
                It.IsAny<ArtistService>(),
                mockGetUserAsyncInUserManager(),
                It.IsAny<SignInManager<User>>());

            // Act
            var result = _sut.Discovery() as ViewResult;

            // Assert
            ((User)result?.ViewData["User"])?.ShouldBe(testUser);
        }

        [Fact]
        public async Task WhenINavigateToSelection_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            const string city = "New York";
            var _sut = new SpotkickController(
                mockGetFollowedArtistsInSpotifyService(),
                mockGetArtistsWithEventsUsingAreaCalendarInArtistService(),
                mockGetUserAsyncInUserManager(),
                It.IsAny<SignInManager<User>>());

            // Act
            var result = await _sut.Selection(city) as ViewResult;

            // Assert
            result?.Model.ShouldBeOfType<SelectionViewModel>();
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
                
            var model = result?.Model as SelectionViewModel;
            
            model?.Location.City.ShouldBe(city);
            model?.User.Id.ShouldBe(testUser.Id);
            model?.Artists.ShouldBe(testArtistsList);
        }

        [Fact]
        public async Task WhenIHaveSelectedSomeArtistsAndProceedToCreateAPlaylist_ThenIShouldLandOnThePlaylistPage()
        {
            // Arrange
            var _sut = new SpotkickController(
                mockGetMostPopularTracksAndCreatePlaylistInSpotifyService(),
                mockGetArtistsByIdInArtistService(),
                mockGetUserAsyncInUserManager(),
                It.IsAny<SignInManager<User>>());

            // Act
            var result = await _sut.Playlist(new List<long>(), 1) as ViewResult;

            // Assert
            result?.Model.ShouldBeOfType<PlaylistViewModel>();
            // default view for controller method always has ViewName of null
            result?.ViewName.ShouldBe(null);
        }
    }
}