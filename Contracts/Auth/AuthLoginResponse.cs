using System;
using Dorbit.Identity.Contracts.Tokens;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginResponse
{
    public Guid OtpId { get; set; }
    public AuthMethod LoginStrategy { get; set; }
    
    public TokenResponse Token { get; set; }
}