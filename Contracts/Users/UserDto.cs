using System;
using System.Collections.Generic;
using Dorbit.Framework.Contracts.Users;

namespace Dorbit.Identity.Contracts.Users;

public class UserDto : BaseUserDto
{
    public int Code { get; set; }
    
    public bool HasPassword { get; set; }
    
    public string Cellphone { get; set; }
    public DateTime? CellphoneValidateTime { get; set; }
    
    public string Email { get; set; }
    public DateTime? EmailValidateTime { get; set; }
    
    public string AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorValidateTime { get; set; }
    
    public bool NeedResetPassword { get; set; }
    public bool IsActive { get; set; }
    
    public string Message { get; set; }
    
    public short ActiveTokenCount { get; set; }
    
    public IEnumerable<string> Accesses { get; set; }
}