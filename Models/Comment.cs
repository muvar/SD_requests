using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TicketTracker.Models;

[Table("Comments")]
public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int TicketId { get; set; }

    [Required]
    public int UserId { get; set; }

    [Required]
    [StringLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public bool IsInternal { get; set; } = false;

    // Навигационные свойства
    [ForeignKey("TicketId")]
    public virtual Ticket Ticket { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;
}
