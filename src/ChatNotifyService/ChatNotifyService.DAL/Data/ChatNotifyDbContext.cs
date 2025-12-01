using ChatNotifyService.ABS.IEntities;
using ChatNotifyService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace ChatNotifyService.DAL.Data;

public class ChatNotifyDbContext : DbContext
{
    
    public ChatNotifyDbContext(DbContextOptions<ChatNotifyDbContext> options)
        : base(options)
    {
    }
    
    public ChatNotifyDbContext()
        : base(new DbContextOptions<ChatNotifyDbContext>())
    {
    }
    
    public DbSet<Chat> Chats { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;
    
    public DbSet<MessageRead> MessageReads { get; set; } = null!;
    
    public DbSet<ChatMember> ChatMembers { get; set; } = null!;
    
    public DbSet<SpaceActivityLog> SpaceActivityLogs { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity => 
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.SpaceId)
                .IsRequired();
            entity.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(255);
            entity.HasMany(c => c.Members)
                .WithOne()
                .HasForeignKey(cm => cm.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        
        modelBuilder.HasPostgresEnum<PermissionLevel>("permission_level");

        modelBuilder.Entity<ChatMember>(entity =>
        {
            entity.Property(cm => cm.JoinedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasKey(cm => new { cm.ChatId, cm.MemberId });
            entity.Property(cm => cm.PermissionLevel)
                .HasColumnType("permission_level")
                .HasConversion<int>()
                .HasConversion<string>()
                .HasConversion<PermissionLevel>()
                .IsRequired();
        });
        
        modelBuilder.Entity<Message>(entity => 
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.ChatId)
                .IsRequired();
            entity.Property(m => m.SenderId)
                .IsRequired();
            entity.Property(m => m.Content)
                .IsRequired();
            entity.Property(m => m.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne<Chat>()
                .WithMany()
                .HasForeignKey(m => m.ChatId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<MessageRead>(entity => 
        {
            entity.HasKey(mr => new { mr.MessageId, mr.ReaderId });
            entity.HasOne(mr => mr.Reader).WithMany()
                .HasForeignKey(mr => mr.ReaderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<SpaceActivityLog>(entity =>
        {
            entity.HasKey(sal => sal.Id);
            entity.Property(sal => sal.SpaceId)
                .IsRequired();
            entity.Property(sal => sal.Type)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(sal => sal.Description)
                .IsRequired()
                .HasMaxLength(1000);
            entity.Property(sal => sal.Timestamp)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
        
        base.OnModelCreating(modelBuilder);
    }
}