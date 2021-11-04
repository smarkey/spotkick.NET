using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spotkick.Interfaces.Spotkick;
using Spotkick.Models;

namespace Spotkick.Services.Spotkick
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SpotkickContext _db;

        public UserService(ILogger<UserService> logger, SpotkickContext dbContext)
        {
            _logger = logger;
            _db = dbContext;
        }

        public async Task CreateUser(User user)
        {
            _logger.LogInformation("Creating a user for {DisplayName}", user.DisplayName);
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User> GetUser(int userId)
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", userId);
            return await _db.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUser(string spotifyUserId)
        {
            _logger.LogInformation("Retrieving user with Spotify ID {SpotifyUserId}", spotifyUserId);
            return await _db.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.SpotifyUserId == spotifyUserId);
        }

        public async Task UpdateUser(User user)
        {
            _logger.LogInformation("Updating user with ID {UserId}", user.Id);
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}