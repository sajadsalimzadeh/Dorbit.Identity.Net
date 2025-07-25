using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginRequest
{
    [JsonIgnore]
    public string UserAgent { get; set; }

    public void FillByRequest(HttpRequest request)
    {
        if (request.Headers.TryGetValue("User-Agent", out var userAgent))
        {
            UserAgent = userAgent;
        }
    }
}