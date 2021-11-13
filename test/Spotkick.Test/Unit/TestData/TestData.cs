using System;
using System.Collections.Generic;
using Spotkick.Models;

namespace Spotkick.Test.Unit.TestData
{
    public static class TestData
    {
        public static readonly User testUser = new()
        {
            Id = Guid.Empty.ToString()
        };

        public static readonly IEnumerable<Artist> testArtistsList = new List<Artist>
        {
            new()
            {
                Id = 1,
                SpotifyId = 1.ToString(),
                SongkickId = 1,
                Name = "Test Artist"
            }
        };
    }
}