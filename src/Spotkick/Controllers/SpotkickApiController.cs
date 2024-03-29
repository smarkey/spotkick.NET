using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Swashbuckle.Swagger.Annotations;

namespace Spotkick.Controllers
{
    [ApiController]
    [Route("user/{userId}")]
    public class SpotkickApiController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IUserService _userService;
        private readonly ISpotifyService _spotifyService;

        public SpotkickApiController(
            IArtistService artistService,
            IUserService userService,
            ISpotifyService spotifyService)
        {
            _artistService = artistService;
            _userService = userService;
            _spotifyService = spotifyService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "User.Read", Tags = new[] { "UserEndpoint" })]
        public async Task<ActionResult<User>> GetUserDetails(string userId)
        {
            var user = await _userService.GetUserById(userId);

            if (user == null) return NotFound();

            user.Token = null;
            return Ok(user);
        }

        [HttpGet("artist")]
        [ProducesResponseType(typeof(IEnumerable<Artist>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "Artist.Read", Tags = new[] { "UserArtistEndpoint" })]
        public async Task<ActionResult<IEnumerable<Artist>>> GetFollowedArtists(string userId,
            [FromQuery] string location)
        {
            var user = await _userService.GetUserById(userId);
            var followedArtists = await _spotifyService.GetFollowedArtists(user.SpotifyUserId);
            var result =
                await _artistService.FilterArtistsWithEventsUsingAreaCalendar(followedArtists, new Location(location));

            return Ok(result);
        }

        [HttpPost("playlist")]
        [ProducesResponseType(typeof(List<Playlist>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(OperationId = "Playlist.Create", Tags = new[] { "UserPlaylistEndpoint" })]
        public async Task<ActionResult<Playlist>> CreatePlaylist(string userId,
            [FromBody] CreatePlaylistCommand command)
        {
            var user = await _userService.GetUserById(userId);
            var artists = await _artistService.GetArtistsById(command.ArtistIds);
            var topTracks =
                await _spotifyService.GetMostPopularTracks(artists, user.SpotifyUserId, command.NumberOfTracks);
            var playlist = await _spotifyService.CreatePlaylist(topTracks, user.SpotifyUserId);

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