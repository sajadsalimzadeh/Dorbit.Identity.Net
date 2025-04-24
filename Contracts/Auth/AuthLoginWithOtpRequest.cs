using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithOtpRequest : AuthLoginRequest
{
    public OtpType Type { get; set; }
    public string Receiver { get; set; }
    public string Code { get; set; }
}