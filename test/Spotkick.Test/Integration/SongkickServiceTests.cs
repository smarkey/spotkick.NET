using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Services;
using Xunit;

namespace Spotkick.Test.Integration
{
    public class SongkickServiceTests
    {
        private readonly SongkickService _sut;

        public SongkickServiceTests()
        {
            _sut = new SongkickService(It.IsAny<ILogger>(), It.IsAny<SpotkickContext>(), It.IsAny<ISpotkickService>());
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
        public async Task ShouldBeAbleToRetrieveEventsForArtist()
        {
            // Arrange + Act
            var events = await _sut.GetEventsForArtist("Foals");

            // Assert
            events.Count.ShouldBeGreaterThan(0);
        }
    }
}