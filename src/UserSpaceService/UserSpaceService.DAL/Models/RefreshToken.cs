using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.DAL.Models;

public sealed class RefreshToken : IRefreshToken
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
    public DateTime ExpiresAt { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    
    [Column("revoked_at")]
    public DateTime? RevokedAt { get; set; }
    
    [Column("replaced_by_token")]
    [MaxLength(500)]
    public string? ReplacedByToken { get; set; }
    
    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
    
    IUser IRefreshToken.User
    {
        get => User;
        set => User = (User)value;
    }
    
    [NotMapped]
    public bool IsActive => RevokedAt == null && ExpiresAt > DateTime.UtcNow;
}