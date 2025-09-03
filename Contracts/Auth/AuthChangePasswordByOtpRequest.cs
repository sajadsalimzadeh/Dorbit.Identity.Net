using System;
using System.Text.Json.Serialization;
using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthChangePasswordByOtpRequest
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string NewPassword { get; set; }

    public OtpValidationRequest OtpValidation { get; set; }
}