using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Models.Spotify;
using Spotkick.Models.Spotify.Track;
using Spotkick.Services.Spotify;
using static Spotkick.Test.Unit.TestData.TestData;

namespace Spotkick.Test.Unit.Mocks
{
    public static class Mocks
    {
        public static UserManager<User> mockGetUserAsyncInUserManager()
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

        public static SignInManager<User> mockSignInAsyncInSignInManager()
        {
            var userManagerMock =
                new Mock<UserManager<User>>(new Mock<IUserStore<User>>().Object, null, null, null, null, null, null,
                    null, null);

            var signInManagerMockWithInteractions = new Mock<SignInManager<User>>(
                userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null, null, null, null);

            signInManagerMockWithInteractions
                .Setup(service => service.SignInAsync(testUser, true, It.IsAny<string>()));

            return signInManagerMockWithInteractions.Object;
        }

        public static IArtistService mockGetArtistsWithEventsUsingAreaCalendarInArtistService()
        {
            var artistServiceMockWithInteractions = new Mock<IArtistService>();

            artistServiceMockWithInteractions
                .Setup(service =>
                    service.FilterArtistsWithEventsUsingAreaCalendar(testArtistsList, It.IsAny<Location>()))
                .ReturnsAsync(testArtistsList);

            return artistServiceMockWithInteractions.Object;
        }

        public static IArtistService mockGetArtistsByIdInArtistService()
        {
            var artistServiceMockWithInteractions = new Mock<IArtistService>();

            artistServiceMockWithInteractions
                .Setup(service => service.GetArtistsById(It.IsAny<List<long>>()))
                .ReturnsAsync(testArtistsList);

            return artistServiceMockWithInteractions.Object;
        }

        public static ISpotifyService mockGetMostPopularTracksAndCreatePlaylistInSpotifyService()
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

        public static ISpotifyService mockAuthorizeUrlInSpotifyService()
        {
            var config = Options.Create(new SpotifyConfig
            {
                AuthorizeUrl = new Uri("https://stub.api/authorize"),
                ClientId = "123",
                CallbackUrl = new Uri("https://stub.callback.api/Spotkick/Callback")
            });

            var spotifyServiceMockWithInteractions = new SpotifyService(It.IsAny<ILogger<SpotifyService>>(),
                It.IsAny<IUserService>(),
                It.IsAny<IArtistService>(), config);

            return spotifyServiceMockWithInteractions;
        }
        
        public static ISpotifyService mockAuthorizeUserInSpotifyService()
        {
            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();

            spotifyServiceMockWithInteractions
                .Setup(service => service.AuthorizeUser(It.IsAny<string>()))
                .ReturnsAsync(testUser);

            return spotifyServiceMockWithInteractions.Object;
        }
        
        public static ISpotifyService mockGetFollowedArtistsInSpotifyService()
        {
            var spotifyServiceMockWithInteractions = new Mock<ISpotifyService>();

            spotifyServiceMockWithInteractions
                .Setup(service => service.GetFollowedArtists(It.IsAny<string>()))
                .ReturnsAsync(testArtistsList);

            return spotifyServiceMockWithInteractions.Object;
        }
    }
}