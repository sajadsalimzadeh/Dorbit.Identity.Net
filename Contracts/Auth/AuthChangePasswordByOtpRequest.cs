using System;
using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthChangePasswordByOtpRequest
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string Receiver { get; set; }
    public string Code { get; set; }
    public string NewPassword { get; set; }
}