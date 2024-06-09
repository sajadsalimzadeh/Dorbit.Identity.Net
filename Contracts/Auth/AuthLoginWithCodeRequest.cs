using System;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithCodeRequest
{
    public Guid OtpId { get; set; }
    public AuthMethod LoginStrategy { get; set; }
    public string UserAgent { get; set; }
    public string Code { get; set; }
}