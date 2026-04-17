using Microsoft.EntityFrameworkCore;
using UserSpaceService.ABS.Models;

namespace UserSpaceService.DAL.Data;


public class UserSpaceDbContext : DbContext
{
    public UserSpaceDbContext(DbContextOptions<UserSpaceDbContext> options)
        : base(options)
    {
    }
    
    public UserSpaceDbContext() : base(new DbContextOptions<UserSpaceDbContext>())
    {
    }
    
    private const int MaxStringLength = 255;
    
    public DbSet<ExternalLogin> ExternalLogins { get; set; } = null!;
    public DbSet<SpaceRole> SpaceRoles { get; set; } = null!;
    public DbSet<SpaceMember> SpaceMembers { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
    public DbSet<Space> Spaces { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Space
        modelBuilder.Entity<Space>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(MaxStringLength);
            entity.HasMany(s => s.Members).WithOne(sm => sm.Space)
                .HasForeignKey(sm => sm.SpaceId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(s => s.Roles).WithOne(e => e.Space)
                .HasForeignKey(e => e.SpaceId).OnDelete(DeleteBehavior.Cascade);
        });

        // User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(MaxStringLength);
            entity.Property(e => e.NormalizedUsername).HasMaxLength(MaxStringLength);
            entity.Property(e => e.Email).HasMaxLength(MaxStringLength);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(MaxStringLength);
            entity.Property(e => e.PasswordHash).HasMaxLength(MaxStringLength);
            entity.Property(e => e.SecurityStamp).HasMaxLength(MaxStringLength);
            entity.HasMany(u => u.ExternalLogins).WithOne(el => el.User)
                .HasForeignKey(el => el.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasMany(u => u.SpaceMemberships).WithOne(sm => sm.User)
                .HasForeignKey(sm => sm.UserId).OnDelete(DeleteBehavior.Cascade);
            
            entity.HasIndex(e => e.NormalizedEmail).IsUnique();
            entity.HasIndex(e => e.NormalizedUsername).IsUnique();
        });
        
        // enum Provider
        modelBuilder.HasPostgresEnum(
            null, "provider",
            ["google", "microsoft"]
        );
        
        // ExternalLogin
        modelBuilder.Entity<ExternalLogin>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Provider)
                .HasConversion<string>()
                .HasColumnType("provider");
            entity.Property(e => e.ProviderKey).HasMaxLength(MaxStringLength);
        });

        // SpaceRole
        modelBuilder.Entity<SpaceRole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(MaxStringLength);
            entity.HasMany(sr => sr.Members).WithOne(sm => sm.Role)
                .HasForeignKey(sm => sm.RoleId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(sr => sr.RolePermissions)
                .HasConversion<long>();
        });

        // SpaceMember
        modelBuilder.Entity<SpaceMember>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.SpaceUsername).HasMaxLength(MaxStringLength);
            entity.HasIndex(e => e.SpaceId);
            entity.HasIndex(e => e.UserId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
