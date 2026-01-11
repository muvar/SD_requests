using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketTracker.Models;

[Table("TicketHistory")]
public class TicketHistory
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(100)]
    public string Action { get; set; } = string.Empty;

    [StringLength(500)]
    public string? OldValue { get; set; }

    [StringLength(500)]
    public string? NewValue { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    // Навигационные свойства
    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
