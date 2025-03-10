using Microsoft.EntityFrameworkCore;
using ServerStudy.Models;

namespace ServerStudy.DataBase;

public class ApplicationDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Clan> Clans { get; set; }
    public DbSet<ChatRoom> ChatRooms { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Match> Matches { get; set; }
    public DbSet<Friendship> Friendships { get; set; }
    public DbSet<Mail> Mails { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 클랜과 리더 간의 관계 설정 (일대일)
        modelBuilder.Entity<Clan>()
            .HasOne(c => c.Leader)
            .WithMany()
            .HasForeignKey(c => c.LeaderId)
            .OnDelete(DeleteBehavior.Restrict);

        // 클랜과 사용자 간의 관계 설정 (다대일)
        modelBuilder.Entity<User>()
            .HasOne(u => u.Clan)
            .WithMany(c => c.Users)
            .HasForeignKey(u => u.ClanId)
            .OnDelete(DeleteBehavior.SetNull);
        
        modelBuilder.Entity<User>()
            .HasOne(m => m.Clan)
            .WithMany(c => c.Users)
            .HasForeignKey(m => m.ClanId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Requester)
            .WithMany()
            .HasForeignKey(f => f.RequesterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Addressee)
            .WithMany()
            .HasForeignKey(f => f.AddresseeId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Mail>()
            .HasOne(m => m.Receiver)
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}