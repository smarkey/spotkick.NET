using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spotkick.Interfaces;
using Spotkick.Models;

namespace Spotkick.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SpotkickDbContext _dbContext;

        public UserService(ILogger<UserService> logger, SpotkickDbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        public async Task CreateUser(User user)
        {
            _logger.LogInformation("Creating a user for {DisplayName}", user.DisplayName);
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<User> GetUser(int userId)
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", userId);
            return await _dbContext.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUser(string spotifyUserId)
        {
            _logger.LogInformation("Retrieving user with Spotify ID {SpotifyUserId}", spotifyUserId);
            return await _dbContext.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.SpotifyUserId == spotifyUserId);
        }

        public async Task UpdateUser(User user)
        {
            _logger.LogInformation("Updating user with ID {UserId}", user.Id);
            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();
        }
    }
}