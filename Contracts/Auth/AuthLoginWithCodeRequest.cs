using System;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithCodeRequest
{
    public Guid OtpId { get; set; }
    public LoginStrategy LoginStrategy { get; set; }
    public string UserAgent { get; set; }
    public string Code { get; set; }
}