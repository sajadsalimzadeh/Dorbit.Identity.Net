using System;
using System.Collections.Generic;
using System.Security.Claims;
using Dorbit.Framework.Contracts.Abstractions;
using Dorbit.Framework.Contracts.Identities;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Contracts.Users;

public class UserDto : IUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    
    public int Code { get; set; }
    public bool HasPassword { get; set; }
    
    public string Cellphone { get; set; }
    public DateTime? CellphoneValidateTime { get; set; }
    
    public string Email { get; set; }
    public DateTime? EmailValidateTime { get; set; }
    
    public string AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorValidateTime { get; set; }
    
    public string Thumbnail { get; set; }
    
    public bool NeedResetPassword { get; set; }
    public UserStatus Status { get; set; }
    
    public string Message { get; set; }
    
    public short MaxTokenCount { get; set; }

    public DateTime CreationTime { get; set; }
    
    public List<string> Accessibility { get; set; }
    public List<string> FirebaseTokens { get; set; }
    public List<ClaimDto> Claims { get; set; }
    
    public IEntity Profile { get; set; }
    
    public Guid GetId()
    {
        return Id;
    }

    public string GetUsername()
    {
        return Username;
    }

    public bool IsActive()
    {
        return Status == UserStatus.Active;
    }

    public List<string> GetFirebaseTokens()
    {
        return FirebaseTokens;
    }
}