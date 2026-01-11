using Microsoft.EntityFrameworkCore;
using TicketTracker.Models;

namespace TicketTracker.Data;

public class TicketTrackerContext : DbContext
{
    public TicketTrackerContext(DbContextOptions<TicketTrackerContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<TicketHistory> TicketHistory { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связей для User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Login).IsUnique();
            entity.Property(e => e.Role).HasConversion<int>();
        });

        // Настройка связей для Ticket
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.Property(e => e.Status).HasConversion<int>();
            entity.Property(e => e.Priority).HasConversion<int>();

            entity.HasOne(d => d.CreatedBy)
                .WithMany(p => p.CreatedTickets)
                .HasForeignKey(d => d.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(d => d.AssignedTo)
                .WithMany(p => p.AssignedTickets)
                .HasForeignKey(d => d.AssignedToId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Настройка связей для Comment
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasOne(d => d.Ticket)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.Comments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Настройка связей для TicketHistory
        modelBuilder.Entity<TicketHistory>(entity =>
        {
            entity.HasOne(d => d.Ticket)
                .WithMany(p => p.History)
                .HasForeignKey(d => d.TicketId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.TicketHistories)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Начальные данные
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Создание администратора по умолчанию
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1,
                Login = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), // В реальном приложении используйте более сложный пароль
                FullName = "Системный администратор",
                Role = UserRole.Administrator,
                IsActive = true,
                CreatedAt = DateTime.Now
            }
        );
    }
}
