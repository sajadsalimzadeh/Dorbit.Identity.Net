using Dorbit.Identity.Contracts.Auth;
using Microsoft.AspNetCore.Http;

namespace Dorbit.Identity.Extensions;

public static class AuthExtensions
{
    public static void FillByRequest(this IAuthLoginRequest authLoginRequest, HttpRequest request)
    {
        if (request.Headers.TryGetValue("User-Agent", out var userAgent))
        {
            authLoginRequest.UserAgent = userAgent;
        }
    }
}