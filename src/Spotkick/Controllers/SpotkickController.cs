using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Spotkick.Interfaces;
using Spotkick.Interfaces.Spotify;
using Spotkick.Models;
using Spotkick.Models.Songkick.Event;

namespace Spotkick.Controllers
{
    public class SpotkickController : Controller
    {
        private readonly ISpotifyService _spotifyService;
        private readonly IArtistService _artistService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public SpotkickController(
            IArtistService artistService,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ISpotifyService spotifyService)
        {
            _artistService = artistService;
            _userManager = userManager;
            _signInManager = signInManager;
            _spotifyService = spotifyService;
        }

        public IActionResult Index() => View();

        public RedirectResult Sso() => Redirect(_spotifyService.AuthorizeUrl());

        public async Task<RedirectResult> Callback([FromQuery] string code)
        {
            var user = await _spotifyService.AuthorizeUser(code);
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
            var followedArtists = await _spotifyService.GetFollowedArtists(user.SpotifyUserId);
            var followedArtistsWithEvents =
                await _artistService.FilterArtistsWithEventsUsingAreaCalendar(followedArtists, new Location(location));

            ViewData["Artists"] = followedArtistsWithEvents;
            ViewData["UserId"] = user.Id;
            ViewData["Location"] = location;

            return View();
        }

        [Authorize]
        public async Task<IActionResult> Playlist(List<long> artistIds, int numberOfTracks)
        {
            var user = await _userManager.GetUserAsync(User);
            var artists = await _artistService.GetArtistsById(artistIds);
            var topTracks = await _spotifyService.GetMostPopularTracks(artists, user.SpotifyUserId, numberOfTracks);

            ViewData["Playlist"] = await _spotifyService.CreatePlaylist(topTracks, user.SpotifyUserId);

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}