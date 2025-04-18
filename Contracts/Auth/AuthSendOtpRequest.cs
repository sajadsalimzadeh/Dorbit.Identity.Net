using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthSendOtpRequest
{
    public OtpType Type { get; set; }
    public string Receiver { get; set; }
}