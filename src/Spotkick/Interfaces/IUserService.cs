using System.Threading.Tasks;
using Spotkick.Models;

namespace Spotkick.Interfaces
{
    public interface IUserService
    {
        Task CreateUser(User user);
        Task<User> GetUserById(string userId);
        Task<User> GetUserBySpotifyId(string spotifyUserId);
        Task UpdateUser(User user);
    }
}