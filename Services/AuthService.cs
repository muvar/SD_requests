using Microsoft.EntityFrameworkCore;
using TicketTracker.Data;
using TicketTracker.Models;

namespace TicketTracker.Services;

public class AuthService : IAuthService
{
    private readonly TicketTrackerContext _context;
    private User? _currentUser;

    public AuthService(TicketTrackerContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string login, string password)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login && u.IsActive);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                user.LastLoginAt = DateTime.Now;
                await _context.SaveChangesAsync();
                
                _currentUser = user;
                return user;
            }

            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<bool> LogoutAsync()
    {
        _currentUser = null;
        return await Task.FromResult(true);
    }

    public async Task<User?> GetCurrentUserAsync()
    {
        return await Task.FromResult(_currentUser);
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        return await Task.FromResult(_currentUser != null);
    }

    public async Task<bool> HasRoleAsync(UserRole role)
    {
        return await Task.FromResult(_currentUser?.Role == role);
    }
}
