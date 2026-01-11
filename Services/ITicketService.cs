using TicketTracker.Models;

namespace TicketTracker.Services;

public interface ITicketService
{
    Task<IEnumerable<Ticket>> GetAllTicketsAsync();
    Task<IEnumerable<Ticket>> GetTicketsByUserAsync(int userId);
    Task<IEnumerable<Ticket>> GetAssignedTicketsAsync(int userId);
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync();
    Task<Ticket?> GetTicketByIdAsync(int id);
    Task<Ticket> CreateTicketAsync(Ticket ticket);
    Task<bool> UpdateTicketAsync(Ticket ticket);
    Task<bool> DeleteTicketAsync(int id);
    Task<bool> AssignTicketAsync(int ticketId, int userId);
    Task<bool> CloseTicketAsync(int ticketId, string resolution);
    Task<bool> UpdateTicketStatusAsync(int ticketId, TicketStatus status);
    Task<IEnumerable<Comment>> GetTicketCommentsAsync(int ticketId);
    Task<Comment> AddCommentAsync(Comment comment);
    Task<IEnumerable<TicketHistory>> GetTicketHistoryAsync(int ticketId);
    Task AddHistoryEntryAsync(int ticketId, int userId, string action, string? oldValue = null, string? newValue = null, string? description = null);
}
