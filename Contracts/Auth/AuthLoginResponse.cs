using System;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Tokens;

namespace Dorbit.Identity.Models.Auth;

public class AuthLoginResponse
{
    public Guid OtpId { get; set; }
    public UserLoginStrategy UserLoginStrategy { get; set; }
    
    public TokenResponse Token { get; set; }
}