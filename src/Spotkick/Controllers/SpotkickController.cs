using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;
using Spotkick.Models.Spotify;
using Spotkick.Services.Spotify;

namespace Spotkick.Controllers
{
    public class SpotkickController : Controller
    {
        public ISpotifyService SpotifyService;
        private readonly IArtistService _artistService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SpotkickController(
            ILogger<SpotkickController> logger,
            IUserService userService,
            IArtistService artistService,
            IOptions<SpotifyConfig> spotifyConfig,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _artistService = artistService;
            _userManager = userManager;
            _signInManager = signInManager;
            SpotifyService = new SpotifyService(logger, userService, artistService, spotifyConfig);
        }

        public IActionResult Index() => View();

        public RedirectResult Sso() => Redirect(SpotifyService.AuthenticationUrl());

        public async Task<RedirectResult> Callback([FromQuery] string code)
        {
            var user = await SpotifyService.AuthenticateUser(code);
            await _signInManager.SignInAsync(user, true);
            return Redirect("Discovery");
        }

        [Authorize]
        public async Task<IActionResult> Discovery()
        {
            ViewData["User"] = await _userManager.GetUserAsync(User);
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Selection([FromQuery] string location)
        {
            var user = await _userManager.GetUserAsync(User);
            ViewData["Artists"] =
                await _artistService.GetFollowedArtistsWithEventsUsingAreaCalendar(user.SpotifyUserId,
                    new Location(location));
            ViewData["UserId"] = user.Id;
            ViewData["Location"] = location;
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Playlist(List<long> artistIds, int numberOfTracks)
        {
            var user = await _userManager.GetUserAsync(User);
            var artists = await _artistService.GetArtistsById(artistIds);
            var topTracks = await SpotifyService.GetMostPopularTracks(artists, user.SpotifyUserId, numberOfTracks);

            ViewData["Playlist"] = await SpotifyService.CreatePlaylist(topTracks, user.SpotifyUserId);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}