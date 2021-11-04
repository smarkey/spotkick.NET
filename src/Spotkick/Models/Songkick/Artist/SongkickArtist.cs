using System;
using System.Collections.Generic;

namespace Spotkick.Models.Songkick.Artist
{
    public class SongkickArtist
    {
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Uri { get; set; }
        public List<object> Identifier { get; set; }
        public DateTime? OnTourUntil { get; set; }
        
        public Models.Artist ToSpotkickArtist()
        {
            return new Models.Artist
            {
                SongkickId = Id,
                Name = DisplayName
            };
        }
    }
}