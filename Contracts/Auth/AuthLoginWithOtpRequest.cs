using System.ComponentModel.DataAnnotations;
using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithOtpRequest : IAuthLoginRequest
{
    public string UserAgent { get; set; }

    [Required]
    public OtpValidationRequest OtpValidation { get; set; }
}