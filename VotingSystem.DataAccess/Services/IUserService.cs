using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VotingSystem.DataAccess.Models;

namespace VotingSystem.DataAccess.Services
{
    public interface IUserService
    {
        Task AddUserAsync(User user, string password, Role? role = null);
        Task<(string authToken, string refreshToken, string userId)> LoginAsync(string email, string password);
        Task<(string authToken, string refreshToken, string userId)> RedeemRefreshTokenAsync(string refreshToken);
        Task LogoutAsync();
        Task<User?> GetCurrentUserAsync();
        string? GetCurrentUserId();
        Task<User> GetUserByIdAsync(string id);

    }
}
