using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Models.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : FullEntity
{
    [StringLength(128), Required] public string Name { get; set; }
    [StringLength(32), Required] public string Username { get; set; }

    [StringLength(128), Required] public string Salt { get; set; }
    [StringLength(128)] public string PasswordHash { get; set; }

    [StringLength(20)] public string Cellphone { get; set; }
    public DateTime? CellphoneValidateTime { get; set; }

    [StringLength(64)] public string Email { get; set; }
    public DateTime? EmailValidateTime { get; set; }

    public byte[] AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorValidateTime { get; set; }

    public bool IsTwoFactorAuthenticationEnable { get; set; }
    public bool NeedResetPassword { get; set; }
    public bool IsActive { get; set; } = true;
    [MaxLength(1024)] public string InActiveMessage { get; set; }
    public short ActiveTokenCount { get; set; } = 1;

    [NotMapped] public ClaimsPrincipal Claims { get; set; }
}