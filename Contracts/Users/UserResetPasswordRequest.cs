using System;

namespace Dorbit.Identity.Contracts.Users;

public class UserResetPasswordRequest
{
    public Guid Id { get; set; }
    public string Password { get; set; }
}