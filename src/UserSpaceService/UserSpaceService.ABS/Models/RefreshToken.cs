using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class RefreshToken
{
    [Key]
    [Column("refresh_token_id")]
    public Guid Id { get; set; }
    
    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("token")]
    [MaxLength(500)]
    public string Token { get; set; } = string.Empty;
    
    [Column("expires_at")]
    public DateTimeOffset ExpiresAt { get; set; }
    
    [Column("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
    
    [Column("revoked_at")]
    public DateTimeOffset? RevokedAt { get; set; }
    
    [Column("replaced_by_token")]
    [MaxLength(500)]
    public string? ReplacedByToken { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}
