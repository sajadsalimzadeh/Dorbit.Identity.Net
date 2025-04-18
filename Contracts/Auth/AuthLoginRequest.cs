using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }
}