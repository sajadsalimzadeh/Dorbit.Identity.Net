using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : FullEntity
{
    [Sequence("seq_user_code")] public int Code { get; set; }
    
    [StringLength(128)] 
    public string Name { get; set; }
    [StringLength(32), Required] 
    public string Username { get; set; }

    [StringLength(128), Required] 
    public string Salt { get; set; }
    [StringLength(128)] 
    public string PasswordHash { get; set; }

    [StringLength(20)] 
    public string Cellphone { get; set; }
    public DateTime? CellphoneValidateTime { get; set; }

    [StringLength(64)] 
    public string Email { get; set; }
    public DateTime? EmailValidateTime { get; set; }
    
    [StringLength(1024)] 
    public string AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorValidateTime { get; set; }

    [StringLength(512)] 
    public string Thumbnail { get; set; }
    
    public bool NeedResetPassword { get; set; }
    
    public bool IsActive { get; set; } = true;

    [MaxLength(1024)]
    public string Message { get; set; }
    
    public short ActiveTokenCount { get; set; } = 1;

    [NotMapped] public ClaimsPrincipal Claims { get; set; }
}