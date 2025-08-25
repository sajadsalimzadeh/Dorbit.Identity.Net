using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Users;

public class UserConfirmCodeDto
{
    public OtpType Type { get; set; }
    public string Code { get; set; }
}