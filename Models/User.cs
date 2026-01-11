using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketTracker.Models;

[Table("Users")]
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Login { get; set; } = string.Empty;

    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? LastLoginAt { get; set; }

    // Навигационные свойства
    public virtual ICollection<Ticket> CreatedTickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Ticket> AssignedTickets { get; set; } = new List<Ticket>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<TicketHistory> TicketHistories { get; set; } = new List<TicketHistory>();
}
