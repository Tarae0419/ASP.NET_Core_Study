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
    }
}