using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Spotkick.Models.Bandsintown
{
    public class Event
    {
        public string Id { get; set; }
        public Artist Artist { get; set; }
        public string Url { get; set; }
        public DateTime OnSaleDateTime { get; set; }
        public DateTime DateTime { get; set; }
        public string Description { get; set; }
        public Venue Venue { get; set; }
        public List<Offer> Offers { get; set; }
        [NotMapped]
        public List<string> Lineup { get; set; }
    }
}
