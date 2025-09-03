using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthForgetPasswordRequest
{
    public string Password { get; set; }

    public OtpValidationRequest OtpValidation { get; set; }
}