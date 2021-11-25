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
            ISpotifyService spotifyService,
            IArtistService artistService,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _spotifyService = spotifyService;
            _artistService = artistService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index() => View();

        public RedirectResult Sso() => Redirect(_spotifyService.AuthorizeUrl());

        public async Task<RedirectResult> Callback(string code)
        {
            var user = await _spotifyService.AuthorizeUser(code);
            await _signInManager.SignInAsync(user, true);

            return Redirect("Discovery");
        }

        [Authorize]
        public IActionResult Discovery() => View();

        [Authorize]
        public async Task<IActionResult> Selection(string city)
        {
            var location = new Location(city);
            var user = await _userManager.GetUserAsync(User);
            var followedArtists = await _spotifyService.GetFollowedArtists(user.SpotifyUserId);
            var followedArtistsWithEvents =
                await _artistService.FilterArtistsWithEventsUsingAreaCalendar(followedArtists, location);

            var model = new SelectionViewModel
            {
                Artists = followedArtistsWithEvents,
                User = user,
                Location = location
            };

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Playlist(List<long> artistIds, int numberOfTracks)
        {
            var user = await _userManager.GetUserAsync(User);
            var artists = await _artistService.GetArtistsById(artistIds);
            var topTracks = await _spotifyService.GetMostPopularTracks(artists, user.SpotifyUserId, numberOfTracks);

            var model = new PlaylistViewModel
            {
                Playlist = await _spotifyService.CreatePlaylist(topTracks, user.SpotifyUserId)
            };

            return View(model);
        }

        public IActionResult Error() => View(new ErrorViewModel
            { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}