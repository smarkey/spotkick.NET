using System.ComponentModel.DataAnnotations;

namespace Spotkick.Models.Bandsintown
{
    public class Offer
    {
        public int Id { get; set; }
        private string Type { get; set; }
        private string Url { get; set; }
        private string Status { get; set; }
    }
}
