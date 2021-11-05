using System.Threading.Tasks;
using Spotkick.Models;

namespace Spotkick.Interfaces
{
    public interface IUserService
    {
        Task CreateUser(User user);
        Task<User> GetUser(int userId);
        Task<User> GetUser(string spotifyUserId);
        Task UpdateUser(User user);
    }
}