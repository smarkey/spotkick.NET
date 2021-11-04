using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Services;
using Swashbuckle.Swagger.Annotations;

namespace Spotkick.Controllers
{
    [ApiController]
    [Route("user/{userId:int}")]
    public class SpotkickApiController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IUserService _userService;
        private readonly SpotifyService _spotifyService;

        public SpotkickApiController(ILogger<SpotkickApiController> logger, IArtistService artistService,
            IUserService userService, IOptions<SpotifyConfig> config)
        {
            _artistService = artistService;
            _userService = userService;
            _spotifyService = new SpotifyService(logger, _userService, _artistService, config);
        }

        [HttpGet]
        [SwaggerOperation(OperationId = "User.Read", Tags = new[] { "UserEndpoint" })]
        public async Task<ActionResult<User>> GetUserDetails(int userId)
        {
            var result = await _userService.GetUser(userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("artist")]
        [SwaggerOperation(OperationId = "Artist.Read", Tags = new[] { "UserArtistEndpoint" })]
        public async Task<ActionResult<IEnumerable<Artist>>> GetFollowedArtists(int userId, [FromQuery] string location)
        {
            var result =
                await _artistService.GetFollowedArtistsWithEventsUsingAreaCalendar(userId, new Location(location));

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("playlist")]
        [SwaggerOperation(OperationId = "Playlist.Create", Tags = new[] { "UserPlaylistEndpoint" })]
        public async Task<ActionResult<Playlist>> CreatePlaylist(int userId, [FromBody] CreatePlaylistCommand command)
        {
            var artists = await _artistService.GetArtistsById(command.ArtistIds);
            var topTracks = await _spotifyService.GetMostPopularTracks(artists, userId, command.NumberOfTracks);
            var playlist = await _spotifyService.CreatePlaylist(topTracks, userId);

            return Ok(playlist);
        }
    }

    public class CreatePlaylistCommand
    {
        public IEnumerable<long> ArtistIds { get; set; }
        public int NumberOfTracks { get; set; }
    }
}