using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spotkick.Data;
using Spotkick.Interfaces;
using Spotkick.Models;

namespace Spotkick.Services
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly SpotkickDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public UserService(ILogger<UserService> logger, SpotkickDbContext dbContext, UserManager<User> userManager)
        {
            _logger = logger;
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task CreateUser(User user)
        {
            _logger.LogInformation("Creating a user for {DisplayName}", user.DisplayName);
            var result = await _userManager.CreateAsync(user, "Te5ter!");

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.ToString());
            }
        }

        public async Task<User> GetUserById(string userId)
        {
            _logger.LogInformation("Retrieving user with ID {UserId}", userId);
            return await _userManager.Users
                .Include(u => u.Token)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User> GetUserBySpotifyId(string spotifyUserId)
        {
            _logger.LogInformation("Retrieving user with Spotify ID {SpotifyUserId}", spotifyUserId);
            return await _userManager.Users
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