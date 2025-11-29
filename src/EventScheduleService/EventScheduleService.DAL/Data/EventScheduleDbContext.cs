using EventScheduleService.DAL.Models;
using Microsoft.EntityFrameworkCore;
namespace EventScheduleService.DAL.Data;

public class EventScheduleDbContext : DbContext
{
    public EventScheduleDbContext(
        DbContextOptions<EventScheduleDbContext> options
        )
        : base(options)
    {
    }
    
    public EventScheduleDbContext()
        : base(new DbContextOptions<EventScheduleDbContext>())
    {
    }
    
    public DbSet<EventCategory> EventCategories { get; set; } = null!;
    
    public DbSet<EventTag> EventTags { get; set; } = null!;
    
    public DbSet<SoloEvent> SoloEvents { get; set; } = null!;
    
    public DbSet<RegularEvent> RegularEvents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EventCategory
        modelBuilder.Entity<EventCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
            entity.HasMany(ec => ec.SoloEvents).WithOne(se => se.Category)
                .HasForeignKey(se => se.CategoryId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(ec => ec.RegularEvents).WithOne(re => re.Category)
                .HasForeignKey(re => re.CategoryId).OnDelete(DeleteBehavior.Cascade);
        });
        
        // EventTag
        modelBuilder.Entity<EventTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Color).HasMaxLength(255);
            entity.HasMany(et => et.SoloEvents).WithMany(se => se.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "SoloEventEventTag",
                    j => j.HasOne<SoloEvent>().WithMany()
                        .HasForeignKey("SoloEventId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<EventTag>().WithMany()
                        .HasForeignKey("EventTagId").OnDelete(DeleteBehavior.Cascade)
                );
            entity.HasMany(et => et.RegularEvents).WithMany(re => re.Tags)
                .UsingEntity<Dictionary<string, object>>(
                    "RegularEventEventTag",
                    j => j.HasOne<RegularEvent>().WithMany()
                        .HasForeignKey("RegularEventId").OnDelete(DeleteBehavior.Cascade),
                    j => j.HasOne<EventTag>().WithMany()
                        .HasForeignKey("EventTagId").OnDelete(DeleteBehavior.Cascade)
                );
        });
        
        // SoloEvent
        modelBuilder.Entity<SoloEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(255);
        });
        
        // enum Day
        modelBuilder.HasPostgresEnum(
            null, "day",
            ["monday", "tuesday", "wednesday", "thursday", "friday", "saturday", "sunday"]
        );
        
        // enum Frequency
        modelBuilder.HasPostgresEnum(
            null, "frequency",
            ["weekly", "biweekly", "triweekly", "monthly"]
        );

        // RegularEvent
        modelBuilder.Entity<RegularEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Day)
                .HasConversion<string>()
                .HasColumnType("day");
            entity.Property(e => e.Frequency)
                .HasConversion<string>()
                .HasColumnType("frequency");
        });

        base.OnModelCreating(modelBuilder);
    }
}