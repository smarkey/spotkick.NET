using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Spotkick.Models;
using Spotkick.Models.Songkick;
using Spotkick.Models.Songkick.Event;
using Spotkick.Services.Songkick;
using Xunit;

namespace Spotkick.Test.Integration
{
    public class SongkickServiceTests
    {
        private readonly SongkickService _sut;

        public SongkickServiceTests()
        {
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.Development.json")
                .Build()
                .GetSection("Songkick")
                .Get<SongkickConfig>();

            _sut = new SongkickService(
                new Mock<ILogger<SongkickService>>().Object,
                Options.Create(config));
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveArtist()
        {
            // Arrange
            const string artistName = "nirvana";

            // Act
            var artist = await _sut.GetArtistByName(artistName);

            // Assert
            artist.Id.ShouldBe(250695);
            artist.DisplayName.ShouldBe("Nirvana");
            artist.Uri.ShouldBe(
                $"http://www.songkick.com/artists/{artist.Id}-{artistName}?utm_source=41036&utm_medium=partner");
            artist.Identifier.ShouldNotBeEmpty();
            artist.OnTourUntil.ShouldBeNull();
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveTheEventsForAnArtist()
        {
            // Arrange + Act
            var events = await _sut.GetEventsForArtistsByArtistName("Foals");

            // Assert
            events.Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [InlineData("Bristol, UK", 24521)]
        [InlineData("Dublin, Ireland", 29314)]
        public async Task ShouldBeAbleToRetrieveTheMetroAreaIdForALocation(string location, int metroAreaId)
        {
            // Arrange + Act
            var result = await _sut.GetMetroAreaId(new Location(location));

            // Assert
            result.ShouldBe(metroAreaId);
        }

        [Theory]
        [InlineData("Bristol, UK")]
        [InlineData("Dublin, Ireland")]
        public async Task ShouldBeAbleToRetrieveTheEventsForALocation(string location)
        {
            // Arrange + Act
            var events = (await _sut.GetEventsForLocation(new Location(location))).ToList();

            // Assert
            events.Count.ShouldBeGreaterThan(0);
            events.FirstOrDefault()?.Location.City.ShouldBe(location);
        }

        [Theory]
        [InlineData("Bristol, UK")]
        [InlineData("Dublin, Ireland")]
        public async Task ShouldBeAbleToRetrieveTheArtistsPerformingInALocation(string location)
        {
            // Arrange + Act
            var artists = await _sut.GetArtistsWithEventsInLocation(new Location(location));

            // Assert
            artists?.Count().ShouldBeGreaterThan(0);
        }

        [Theory]
        [InlineData("Nirvana", 0)]
        [InlineData("Anthrax", 1)]
        public async Task ShouldBeAbleToRetrieveArtistsWithUpcomingEvents(string artistName, int artistsWithEventsCount)
        {
            // Arrange + Act
            var artist = await _sut.GetArtistByName(artistName);
            var artists = await _sut.FilterArtistsWithEventsInLocation(
                new List<Artist>
                {
                    artist.ToSpotkickArtist()
                },
                new Location("Bristol, UK"));

            // Assert
            artists.Count().ShouldBe(artistsWithEventsCount);
        }
    }
}