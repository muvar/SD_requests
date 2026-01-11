using Microsoft.EntityFrameworkCore;
using TicketTracker.Data;
using TicketTracker.Models;

namespace TicketTracker.Services;

public class TicketService : ITicketService
{
    private readonly TicketTrackerContext _context;

    public TicketService(TicketTrackerContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId)
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.CreatedById == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(int userId)
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.AssignedToId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync()
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Where(t => t.Status != TicketStatus.Closed)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.CreatedBy)
            .Include(t => t.AssignedTo)
            .Include(t => t.Comments)
                .ThenInclude(c => c.User)
            .Include(t => t.History)
                .ThenInclude(h => h.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Ticket> CreateTicketAsync(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.Now;
        ticket.Status = TicketStatus.New;
        
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        await AddHistoryEntryAsync(ticket.Id, ticket.CreatedById, "Создана заявка", null, null, $"Заявка создана: {ticket.Title}");

        return ticket;
    }

    public async Task<bool> UpdateTicketAsync(Ticket ticket)
    {
        try
        {
            ticket.UpdatedAt = DateTime.Now;
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteTicketAsync(int id)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
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

    public async Task<bool> AssignTicketAsync(int ticketId, int userId)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null)
            {
                var oldAssignee = ticket.AssignedToId?.ToString() ?? "Не назначен";
                var newAssignee = await _context.Users.FindAsync(userId);
                
                ticket.AssignedToId = userId;
                ticket.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                
                await AddHistoryEntryAsync(ticketId, userId, "Назначение исполнителя", 
                    oldAssignee, newAssignee?.FullName ?? userId.ToString(), 
                    $"Заявка назначена пользователю {newAssignee?.FullName}");
                
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> CloseTicketAsync(int ticketId, string resolution)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null)
            {
                var oldStatus = ticket.Status.ToString();
                ticket.Status = TicketStatus.Closed;
                ticket.Resolution = resolution;
                ticket.ClosedAt = DateTime.Now;
                ticket.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                
                await AddHistoryEntryAsync(ticketId, ticket.AssignedToId ?? ticket.CreatedById, 
                    "Закрытие заявки", oldStatus, "Закрыта", $"Заявка закрыта. Решение: {resolution}");
                
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket != null)
            {
                var oldStatus = ticket.Status.ToString();
                ticket.Status = status;
                ticket.UpdatedAt = DateTime.Now;
                
                await _context.SaveChangesAsync();
                
                await AddHistoryEntryAsync(ticketId, ticket.AssignedToId ?? ticket.CreatedById, 
                    "Изменение статуса", oldStatus, status.ToString(), 
                    $"Статус изменен с {oldStatus} на {status}");
                
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public async Task<IEnumerable<Comment>> GetTicketCommentsAsync(int ticketId)
    {
        return await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TicketId == ticketId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Comment> AddCommentAsync(Comment comment)
    {
        comment.CreatedAt = DateTime.Now;
        _context.Comments.Add(comment);
        await _context.SaveChangesAsync();

        await AddHistoryEntryAsync(comment.TicketId, comment.UserId, "Добавлен комментарий", 
            null, null, $"Добавлен комментарий: {comment.Text.Substring(0, Math.Min(50, comment.Text.Length))}...");

        return comment;
    }

    public async Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId)
    {
        return await _context.TicketHistory
            .Include(h => h.User)
            .Where(h => h.TicketId == ticketId)
            .OrderBy(h => h.CreatedAt)
            .ToListAsync();
    }

    public async Task AddHistoryEntryAsync(int ticketId, int userId, string action, string? oldValue = null, string? newValue = null, string? description = null)
    {
        var historyEntry = new TicketHistory
        {
            TicketId = ticketId,
            UserId = userId,
            Action = action,
            OldValue = oldValue,
            NewValue = newValue,
            Description = description,
            CreatedAt = DateTime.Now
        };

        _context.TicketHistory.Add(historyEntry);
        await _context.SaveChangesAsync();
    }
}
