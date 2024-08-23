using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<UserLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserLike>()
        .HasKey(k => new { k.SourceUserId, k.TargetUserId });

        builder.Entity<UserLike>()
        .HasOne(s => s.SourceUser)
        .WithMany(users => users.LikedUsers)
        .HasForeignKey(s => s.SourceUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>()
        .HasOne(t => t.TargetUser)
        .WithMany(users => users.LikedByOtherUsers)
        .HasForeignKey(t => t.TargetUserId)
        .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Message>()
        .HasOne(x => x.Sender)
        .WithMany(x => x.MessagesSent)
        .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Message>()
        .HasOne(x => x.Receiver)
        .WithMany(x => x.MessagesReceived)
        .OnDelete(DeleteBehavior.Restrict);
    }
}
