using MyAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MyAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<BusLine> BusLines => Set<BusLine>();

    public DbSet<User> Users => Set<User>();

    public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BusLine>(entity =>
        {
            entity.HasOne(busLine => busLine.CreatedByUser)
                .WithMany(user => user.CreatedBusLines)
                .HasForeignKey(busLine => busLine.CreateUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(busLine => busLine.UpdatedByUser)
                .WithMany(user => user.UpdatedBusLines)
                .HasForeignKey(busLine => busLine.UpdateUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<ChatMessage>(entity =>
        {
            entity.Property(message => message.Message)
                .IsRequired()
                .HasMaxLength(4000);

            entity.HasIndex(message => new { message.ConversationUserId, message.SentAt });

            entity.HasOne(message => message.ConversationUser)
                .WithMany()
                .HasForeignKey(message => message.ConversationUserId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasOne(message => message.SenderUser)
                .WithMany()
                .HasForeignKey(message => message.SenderUserId)
                .OnDelete(DeleteBehavior.NoAction);
        });
    }
}
