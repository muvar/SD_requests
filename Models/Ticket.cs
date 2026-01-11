using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketTracker.Models;

[Table("Tickets")]
public class Ticket
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public TicketStatus Status { get; set; } = TicketStatus.New;

    [Required]
    public TicketPriority Priority { get; set; } = TicketPriority.Medium;

    [Required]
    public int CreatedById { get; set; }

    public int? AssignedToId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime? UpdatedAt { get; set; }

    public DateTime? ClosedAt { get; set; }

    [StringLength(500)]
    public string? Resolution { get; set; }

    // Навигационные свойства
    [ForeignKey("CreatedById")]
    public virtual User CreatedBy { get; set; } = null!;

    [ForeignKey("AssignedToId")]
    public virtual User? AssignedTo { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<TicketHistory> History { get; set; } = new List<TicketHistory>();
}
