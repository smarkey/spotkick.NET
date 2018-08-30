using Spotkick.Models.Bandsintown;
using Spotkick.Services;
using WireMock.Matchers.Request;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.ResponseProviders;

namespace Spotkick.Test.Integration.Mocks
{
    internal static class BandsinstownMocks
    {
        public static IRequestMatcher GetArtistsRequest(Artist firstArtist)
        {
            var bandsintownService = new BandsintownService();

            return Request.Create()
                .WithPath($"/artists/{bandsintownService.ReplaceUnsupportedChars(firstArtist.Name)}")
                .WithParam("app_id", "6a83e6706d3f30f6ad4a31e89d8d1e50")
                .UsingGet();
        }

        public static IResponseProvider GetArtistsResponse(Artist firstArtist)
        {
            return Response.Create()
                .WithStatusCode(200)
                .WithHeader("Content-Type", "application/json")
                .WithBody("{" +
                          $"\"id\": {firstArtist.Id}, " +
                          $"\"name\": \"{firstArtist.Name}\", " +
                          $"\"url\": \"{firstArtist.Url}\", " +
                          $"\"image_url\": \"{firstArtist.ImageUrl}\", " +
                          $"\"thumb_url\": \"{firstArtist.ThumbUrl}\", " +
                          $"\"facebook_page_url\": \"{firstArtist.FacebookPageUrl}\", " +
                          $"\"mbid\": \"{firstArtist.Mbid}\", " +
                          $"\"tracker_count\": {firstArtist.TrackerCount}, " +
                          $"\"upcoming_event_count\": {firstArtist.UpcomingEventCount}" +
                          "}");
        }
    }
}
