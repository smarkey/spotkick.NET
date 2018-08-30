using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Spotkick.Models;
using Spotkick.Services;

namespace Spotkick.Controllers
{
    public class SpotkickController : Controller
    {
        private readonly BandsintownService _bandsintownService = new BandsintownService();
        private readonly SpotifyService _spotifyService = new SpotifyService();

        public void Index()
        {
            Response.Redirect(_spotifyService.AuthenticationUrl());
        }

        public IActionResult Callback()
        {
            var authCode = HttpUtility.ParseQueryString(Request.QueryString.Value)["code"];
            ViewData["User"] = _spotifyService.CreateUser(authCode);

            return View();
        }

        public IActionResult Events()
        {
            var userId = int.Parse(HttpUtility.ParseQueryString(Request.QueryString.Value)["userId"]);
            var spotifyArtists = _spotifyService.Artists(userId);
            var bandsintownArtists = _bandsintownService.MatchArtists(spotifyArtists);
            ViewData["EventArtists"] = _bandsintownService.Events(bandsintownArtists, new DateRange()).Select(e => e.Artist);

            return View();
        }

        //public IActionResult MatchArtists()
        //{
        //    ViewData["Artists"] = _bandsintownService.MatchArtists(spotifyArtists);

        //    return View();
        //}

        //public IActionResult TopTracks()
        //{
        //    ViewData["TopTracks"] = _spotifyService.TopTracks(matchingArtists, authToken);

        //    return View();
        //}

        //public IActionResult Events()
        //{
        //    ViewData["Events"] = _bandsintownService.Events(matchingArtists, new DateRange());

        //    return View();
        //}

        //public IActionResult Playlist()
        //{
        //    throw new System.NotImplementedException();
        //}

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
