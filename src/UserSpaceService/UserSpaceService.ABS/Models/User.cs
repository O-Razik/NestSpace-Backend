using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class User
{
    [Key]
    [Column("user_id")]
    public Guid Id { get; set; }
    
    [Column("username")]
    [MaxLength(255)]
    public string Username { get; set; } = string.Empty;
    
    [Column("normalized_username")]
    [MaxLength(255)]
    public string NormalizedUsername { get; set; } = string.Empty;
    
    [Column("email")]
    [MaxLength(255)]
    public string Email { get; set; } = string.Empty;
    
    [Column("normalized_email")]
    [MaxLength(255)]
    public string NormalizedEmail { get; set; } = string.Empty;
    
    [Column("password_hash")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Column("security_stamp")]
    [MaxLength(255)]
    public string SecurityStamp { get; set; } = string.Empty;

    [Column("avatar_url")]
    [MaxLength(512)]
    public string? AvatarUrl { get; set; }

    public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];

    public ICollection<SpaceMember> SpaceMemberships { get; set; } = [];
}
