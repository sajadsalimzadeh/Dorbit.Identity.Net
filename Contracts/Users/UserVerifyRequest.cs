using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Users;

public class UserVerifyRequest
{
    public OtpType Type { get; set; }
    public string Receiver { get; set; }
    public string Code { get; set; }
}