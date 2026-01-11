using Microsoft.EntityFrameworkCore;
using TicketTracker.Data;
using TicketTracker.Models;

namespace TicketTracker.Services;

public class UserService : IUserService
{
    private readonly TicketTrackerContext _context;

    public UserService(TicketTrackerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Where(u => u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetUsersByRoleAsync(UserRole role)
    {
        return await _context.Users
            .Where(u => u.Role == role && u.IsActive)
            .OrderBy(u => u.FullName)
            .ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<User?> GetUserByLoginAsync(string login)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Login == login);
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        user.CreatedAt = DateTime.Now;
        user.IsActive = true;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> UpdateUserAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                // Мягкое удаление - деактивация пользователя
                user.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string newPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeactivateUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = false;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ActivateUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.IsActive = true;
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }
}
