namespace Spotkick.Models.Spotify
{
    public class Token
    {
        public int Id { get; set; }
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long ExpiresIn { get; set; }
    }
}
