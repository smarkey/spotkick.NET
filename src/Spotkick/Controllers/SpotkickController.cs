using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spotkick.Enums;
using Spotkick.Interfaces;
using Spotkick.Models;
using Spotkick.Services;

namespace Spotkick.Controllers
{
    public class SpotkickController : Controller
    {
        public ISpotifyService SpotifyService;
        public ISpotkickService SpotkickService;

        public SpotkickController(ILogger<SpotkickController> logger, SpotkickContext dbContext)
        {
            SpotkickService = new SpotkickService(logger, dbContext);
            SpotifyService = new SpotifyService(logger, dbContext, SpotkickService);
        }

        public RedirectResult Index()
        {
            return Redirect(SpotifyService.AuthenticationUrl());
        }

        public async Task<RedirectResult> Callback([FromQuery] string code)
        {
            var user = await SpotifyService.AuthenticateUser(code);

            return Redirect($"Discovery?userId={user.Id}");
        }

        public async Task<IActionResult> Discovery([FromQuery] int userId)
        {
            ViewData["User"] = await SpotkickService.GetUser(userId);

            return View();
        }

        public async Task<IActionResult> Selection([FromQuery] int userId, [FromQuery] string city,
            [FromQuery] DiscoveryOption discoveryOption)
        {
            var artistsFilteredByCity = await SpotkickService.GetArtistsFilteredByCity(userId, city);

            if (discoveryOption == DiscoveryOption.Default)
            {
                var topTracks = await SpotifyService.GetMostPopularTracks(artistsFilteredByCity, userId);
                var playlist = await SpotifyService.CreatePlaylist(topTracks, userId, $"Gigs in {city}");

                ViewData["Playlist"] = playlist;
                return View("Playlist");
            }

            ViewData["SpotifyArtists"] = artistsFilteredByCity;
            ViewData["UserId"] = userId;
            ViewData["City"] = city;
            return View();
        }

        public async Task<IActionResult> Playlist([FromQuery] int userId, List<long> artistIds)
        {
            var artists = await SpotkickService.GetArtists(artistIds);
            var topTracks = await SpotifyService.GetMostPopularTracks(artists, userId);

            ViewData["Playlist"] = await SpotifyService.CreatePlaylist(topTracks, userId);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}