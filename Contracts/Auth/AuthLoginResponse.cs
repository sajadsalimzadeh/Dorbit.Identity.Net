using System;
using Dorbit.Identity.Contracts.Tokens;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginResponse
{
    public required string CsrfToken { get; set; }
    public required string AccessToken { get; set; }
    public bool IsNeedAuthentication { get; set; }
}