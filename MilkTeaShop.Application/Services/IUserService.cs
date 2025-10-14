using MilkTeaShop.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MilkTeaShop.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> AuthenticateAsync(string username, string password);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
        Task<bool> IsUsernameAvailableAsync(string username);
    }
}