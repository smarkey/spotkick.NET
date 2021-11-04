namespace Spotkick.Models.Songkick.Event
{
    public class Location
    {
        public Location(string city)
        {
            City = city;
        }

        public string City { get; set; }
    }
}