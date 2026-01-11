using TicketTracker.Models;

namespace TicketTracker.Services;

public interface IAuthService
{
    Task<User?> LoginAsync(string login, string password);
    Task<bool> LogoutAsync();
    Task<User?> GetCurrentUserAsync();
    Task<bool> IsAuthenticatedAsync();
    Task<bool> HasRoleAsync(UserRole role);
}
