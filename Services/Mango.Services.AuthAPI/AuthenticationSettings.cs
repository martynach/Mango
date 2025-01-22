namespace dot_net_api;

public class AuthenticationSettings
{
    public string JwtKey { get; set; } = String.Empty;
    
    public string JwtIssuer { get; set; } = String.Empty;
    
    public string JwtAudience { get; set; } = String.Empty;
}