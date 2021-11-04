using System;
using System.ComponentModel.DataAnnotations;

namespace Spotkick.Models
{
    public sealed class Artist
    {
        [Key] public long Id { get; set; }
        public string SpotifyId { get; init; }
        public int? SongkickId { get; set; }
        public string Name { get; init; }

        public override string ToString()
        {
            return $"{Name}, ID: {Id}, Spotify: {SpotifyId}, Songkick: {SongkickId}";
        }

        private bool Equals(Artist other)
        {
            return Id == other.Id &&
                   SpotifyId == other.SpotifyId &&
                   SongkickId == other.SongkickId &&
                   Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Artist)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, SpotifyId, SongkickId, Name);
        }
    }
}