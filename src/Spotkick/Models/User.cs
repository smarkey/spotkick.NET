﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Spotkick.Models.Spotify;

namespace Spotkick.Models
{
    public class User : IUser
    {
        [Key]
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public Token Token { get; set; }
        public List<Artist> FollowedArtists { get; set; }
        public List<Playlist> Playlists { get; set; }
        public string SpotifyUserId { get; set; }
        
        public override string ToString()
        {
            return $"{DisplayName}, ID: {Id}, Spotify: {SpotifyUserId}";
        }
    }

    public interface IUser
    {
    }
}