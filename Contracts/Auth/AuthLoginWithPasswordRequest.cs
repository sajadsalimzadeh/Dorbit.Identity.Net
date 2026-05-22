using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithPasswordRequest : IAuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }
    
    public string Username { get; set; }
    public string Password { get; set; }
}