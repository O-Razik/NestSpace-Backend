using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UserSpaceService.ABS.Models;

public sealed class ExternalLogin
{
    [Key]
    [Column("external_login_id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid UserId { get; set; }
    
    [Column("provider")]
    [MaxLength(255)]
    public Provider Provider { get; set; }
    
    [Column("provider_key")]
    [MaxLength(255)]
    public string ProviderKey { get; set; } = string.Empty;

    [ForeignKey("UserId")]
    public User User { get; set; } = null!;
}

public enum Provider 
{
    Google,
    Microsoft,
}
