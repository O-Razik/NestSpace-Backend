using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UserSpaceService.ABS.IModels;

namespace UserSpaceService.DAL.Models;

public sealed class User : IUser
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
    
    public ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    public ICollection<SpaceMember> SpaceMemberships { get; set; } = new List<SpaceMember>();
    
    ICollection<IExternalLogin> IUser.ExternalLogins
    {
        get => ExternalLogins.Cast<IExternalLogin>().ToList();
        set => ExternalLogins = value.Cast<ExternalLogin>().ToList();
    }

    ICollection<ISpaceMember> IUser.SpaceMemberships
    {
        get => SpaceMemberships.Cast<ISpaceMember>().ToList();
        set => SpaceMemberships = value.Cast<SpaceMember>().ToList();
    }
}