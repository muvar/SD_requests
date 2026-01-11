using TicketTracker.Models;

namespace TicketTracker.Services;

public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role);
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByLoginAsync(string login);
    Task<User> CreateUserAsync(User user, string password);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string newPassword);
    Task<bool> DeactivateUserAsync(int userId);
    Task<bool> ActivateUserAsync(int userId);
}
