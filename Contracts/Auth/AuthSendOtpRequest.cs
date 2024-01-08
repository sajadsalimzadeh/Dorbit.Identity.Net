using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Models.Auth;

public class AuthSendOtpRequest
{
    public string Value { get; set; }
    public UserLoginStrategy LoginStrategy { get; set; }
}