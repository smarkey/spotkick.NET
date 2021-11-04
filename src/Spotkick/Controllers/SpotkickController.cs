using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Services;

namespace Spotkick.Controllers
{
    public class SpotkickController : Controller
    {
        public ISpotifyService SpotifyService;
        private readonly IUserService _userService;
        private readonly IArtistService _artistService;

        public SpotkickController(
            ILogger<SpotkickController> logger,
            SpotkickContext dbContext,
            IUserService userService,
            IArtistService artistService,
            IOptions<SpotifyConfig> spotifyConfig)
        {
            _userService = userService;
            _artistService = artistService;
            SpotifyService = new SpotifyService(logger, userService, artistService, spotifyConfig);
        }

        public IActionResult Index() => View();

        public RedirectResult Sso() => Redirect(SpotifyService.AuthenticationUrl());

        public async Task<RedirectResult> Callback([FromQuery] string code)
        {
            var user = await SpotifyService.AuthenticateUser(code);
            return Redirect($"Discovery?userId={user.Id}");
        }

        public async Task<IActionResult> Discovery([FromQuery] int userId)
        {
            ViewData["User"] = await _userService.GetUser(userId);
            return View();
        }

        public async Task<IActionResult> Selection([FromQuery] int userId, [FromQuery] string location)
        {
            ViewData["Artists"] =
                await _artistService.GetFollowedArtistsWithEventsUsingAreaCalendar(userId, new Location(location));
            ViewData["UserId"] = userId;
            ViewData["Location"] = location;
            return View();
        }

        public async Task<IActionResult> Playlist(int userId, List<long> artistIds, int numberOfTracks)
        {
            var artists = await _artistService.GetArtistsById(artistIds);
            var topTracks = await SpotifyService.GetMostPopularTracks(artists, userId, numberOfTracks);

            ViewData["Playlist"] = await SpotifyService.CreatePlaylist(topTracks, userId);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}