using Spotkick.Models.Bandsintown;
using Spotkick.Services;
using System.Collections.Generic;
using System.Linq;
using Spotkick.Test.Integration.Mocks;
using WireMock.Server;
using Xunit;

namespace Spotkick.Test.Integration
{
    public class BandinstownServiceSpec
    {

        [Fact]
        public void ArtistsInSpotifyShouldBeFoundOnBandsintownUsingTheApi()
        {
            var spotifyArtists = new List<Artist> {new Artist() {Name = "Nirvana", SpotifyId = Faker.Lorem.GetFirstWord()}};
            var bandsintownArtists = new List<Artist>
            {
                new Artist()
                {
                    Id = Faker.RandomNumber.Next(),
                    Name = spotifyArtists[0].Name,
                    Url = $"http://{Faker.Internet.DomainName()}{Faker.Internet.DomainSuffix()}",
                    ImageUrl = $"http://{Faker.Internet.DomainName()}{Faker.Internet.DomainSuffix()}",
                    ThumbUrl = $"http://{Faker.Internet.DomainName()}{Faker.Internet.DomainSuffix()}",
                    FacebookPageUrl = $"http://{Faker.Internet.DomainName()}{Faker.Internet.DomainSuffix()}",
                    Mbid = Faker.Lorem.GetFirstWord(),
                    TrackerCount = Faker.RandomNumber.Next(),
                    UpcomingEventCount = Faker.RandomNumber.Next(),
                    SpotifyId =  spotifyArtists[0].SpotifyId
                }
            };

            var server = FluentMockServer.Start();
            var bandsintownService = new BandsintownService() { _serviceUrl = server.Urls.First() };
            var firstArtist = bandsintownArtists.First();

            server
                .Given(BandsinstownMocks.GetArtistsRequest(firstArtist))
                .RespondWith(BandsinstownMocks.GetArtistsResponse(firstArtist));

            var result = bandsintownService.MatchingArtists(spotifyArtists);

            Assert.Equal(bandsintownArtists.Count(), result.Count());
            Assert.Equal(bandsintownArtists[0].Name, result[0].Name);
            Assert.Equal(spotifyArtists[0].SpotifyId, result[0].SpotifyId);
            Assert.Equal(bandsintownArtists[0].FacebookPageUrl, result[0].FacebookPageUrl);
            Assert.Equal(bandsintownArtists[0].Url, result[0].Url);
            Assert.Equal(bandsintownArtists[0].ImageUrl, result[0].ImageUrl);
            Assert.Equal(bandsintownArtists[0].ThumbUrl, result[0].ThumbUrl);
            Assert.Equal(bandsintownArtists[0].Mbid, result[0].Mbid);
            Assert.Equal(bandsintownArtists[0].TrackerCount, result[0].TrackerCount);
            Assert.Equal(bandsintownArtists[0].UpcomingEventCount, result[0].UpcomingEventCount);
        }
    }
}
