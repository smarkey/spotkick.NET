namespace Spotkick.Models.Bandsintown
{
    public class Venue
    {
        public int Id { get; set; }
        private string Name { get; set; }
        private double Latitude { get; set; }
        private double Longitude { get; set; }
        public string City { get; set; }
        private string Region { get; set; }
        private string Country { get; set; }
    }
}
