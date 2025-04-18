using System;
using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthChangePasswordByPasswordRequest
{
    [JsonIgnore]
    public Guid UserId { get; set; }
    public string Password { get; set; }
    public string NewPassword { get; set; }
}