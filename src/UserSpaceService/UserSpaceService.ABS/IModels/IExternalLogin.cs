namespace UserSpaceService.ABS.IModels;

public enum Provider 
{
    Google,
    Microsoft,
}

public interface IExternalLogin
{
    Guid Id { get; set; }
    
    Provider Provider { get; set; }
    
    string ProviderKey { get; set; }
    
    Guid UserId { get; set; }
    
    IUser User { get; set; }
}
