using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithGoogleRequest : IAuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }
    
    [FromQuery(Name = "code")] public string Code { get; set; }
    [FromQuery(Name = "token")] public string AccessToken { get; set; }
}