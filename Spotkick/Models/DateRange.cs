using System;

namespace Spotkick.Models
{
    public class DateRange
    {
        public DateTime StartDate = DateTime.Now;
        public DateTime EndDate = new DateTime(3000,12,12);

        public string UrlEncodedString()
        {
            return $"&date={this.StartDate.Year}-{this.StartDate.Month}-{this.StartDate.Day}%2C{this.EndDate.Year}-{this.EndDate.Month}-{this.EndDate.Day}";
        }
    }
}
