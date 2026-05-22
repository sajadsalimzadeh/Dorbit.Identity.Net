using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithAppleRequest : IAuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }
    
    public string State { get; set; }
    [FromForm(Name = "code")]
    public string AuthenticationCode { get; set; }
    [FromForm(Name = "id_token")]
    public string IdToken { get; set; }
}