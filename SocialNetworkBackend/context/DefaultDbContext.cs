using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Test2.models;


namespace Test2.context;

public class DefaultDbContext : IdentityDbContext<User, Role, Guid>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Friend> Friends { get; set; }
    public DbSet<FriendRequest> FriendRequests { get; set; }

    public DefaultDbContext()
    {
    }

    public DefaultDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=1232;Database=ivansoloncov;Username=postgres;Password=nvidia");
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Friend>(entity =>
        {
            entity.HasOne(d => d.CurrentUser)
                .WithMany(p => p.CurrentUserFriends)
                .HasForeignKey(d => d.CurrentUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_CurrentUserId");
                

            entity.HasOne(d => d.FriendUser)
                .WithMany(p => p.FriendUserFriends)
                .HasForeignKey(d => d.FriendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Friends_FriendUserId");
        });
        
        modelBuilder.Entity<FriendRequest>(entity =>
        {
            entity.HasOne(d => d.User)
                .WithMany(p => p.UserFriendRequests)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_UserId");
            
            entity.HasOne(d => d.Receiver)
                .WithMany(p => p.ReceiverFriendRequests)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FriendRequests_ReceiverId");
        });
        base.OnModelCreating(modelBuilder);
    }
}

