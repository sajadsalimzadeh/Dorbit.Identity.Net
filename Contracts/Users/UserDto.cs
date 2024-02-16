using System;
using System.Collections.Generic;
using Dorbit.Framework.Contracts.Users;

namespace Dorbit.Identity.Contracts.Users;

public class UserDto : BaseUserDto
{
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public bool IsTwoFactorAuthenticationEnable { get; set; }
    public bool NeedResetPassword { get; set; }
    public bool IsActive { get; set; }
    public string Message { get; set; }
    public IEnumerable<string> Accesses { get; set; }
}