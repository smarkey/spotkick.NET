using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Models.Spotify;
using Spotkick.Services.Spotify;
using Swashbuckle.Swagger.Annotations;

namespace Spotkick.Controllers
{
    [ApiController]
    [Route("user/{userId:int}")]
    public class SpotkickApiController : ControllerBase
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
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "User.Read", Tags = new[] { "UserEndpoint" })]
        public async Task<ActionResult<User>> GetUserDetails(int userId)
        {
            var user = await _userService.GetUser(userId);

            if (user == null)
            {
                return NotFound();
            }

            user.Token = null;
            return Ok(user);
        }

        [HttpGet("artist")]
        [ProducesResponseType(typeof(IEnumerable<Artist>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "Artist.Read", Tags = new[] { "UserArtistEndpoint" })]
        public async Task<ActionResult<IEnumerable<Artist>>> GetFollowedArtists(int userId, [FromQuery] string location)
        {
            var result =
                await _artistService.GetFollowedArtistsWithEventsUsingAreaCalendar(userId, new Location(location));

            return Ok(result);
        }

        [HttpPost("playlist")]
        [ProducesResponseType(typeof(List<Playlist>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "Playlist.Create", Tags = new[] { "UserPlaylistEndpoint" })]
        public async Task<ActionResult<Playlist>> CreatePlaylist(int userId, [FromBody] CreatePlaylistCommand command)
        {
            var artists = await _artistService.GetArtistsById(command.ArtistIds);
            var topTracks = await _spotifyService.GetMostPopularTracks(artists, userId, command.NumberOfTracks);
            var playlist = await _spotifyService.CreatePlaylist(topTracks, userId);

            playlist.OwnedBy = null;

            return Created(playlist.SpotifyId, playlist);
        }
    }

    public class CreatePlaylistCommand
    {
        public IEnumerable<long> ArtistIds { get; set; }
        public int NumberOfTracks { get; set; }
    }
}