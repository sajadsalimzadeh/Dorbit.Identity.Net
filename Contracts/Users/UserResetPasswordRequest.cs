using System;
using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Users;

public class UserResetPasswordRequest
{
    [JsonIgnore]
    public Guid Id { get; set; }

    public string Password { get; set; }

    public bool IsSendMessage { get; set; }
}