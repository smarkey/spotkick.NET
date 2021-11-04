using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Shouldly;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Services;
using Xunit;

namespace Spotkick.Test.Integration
{
    public class SongkickServiceTests
    {
        private readonly SongkickService _sut;

        public SongkickServiceTests()
        {
            _sut = new SongkickService(It.IsAny<ILogger>(), It.IsAny<IArtistService>(),
                new Mock<IOptions<SongkickConfig>>().Object);
        }

        [Fact]
        public async Task ShouldBeAbleToRetrieveArtist()
        {
            // Arrange
            const string artistName = "nirvana";

            // Act
            var artist = await _sut.GetArtist(artistName);

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
            var events = await _sut.GetEventsForArtist("Foals");

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
            var events = await _sut.GetEventsForLocation(new Location(location));

            // Assert
            events?.Count().ShouldBeGreaterThan(0);
            events?.FirstOrDefault().Location.City.ShouldBe(location);
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
    }
}