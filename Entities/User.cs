using System.ComponentModel.DataAnnotations;
using Dorbit.Entities;
using Dorbit.Identity.Enums;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Index(nameof(Username), IsUnique = true)]
public class User : FullEntity
{
    [StringLength(128), Required] public string Name { get; set; }
    [StringLength(32), Required] public string Username { get; set; }
    [StringLength(128)] public string Salt { get; set; }
    [StringLength(128)] public string PasswordHash { get; set; }
    
    [StringLength(20)] public string Cellphone { get; set; }
    public DateTime? CellphoneValidateTime { get; set; }
    public bool CellphoneLoginEnable { get; set; }
    
    [StringLength(20)] public string Email { get; set; }
    public DateTime? EmailValidateTime { get; set; }
    public bool EmailLoginEnable { get; set; }

    public byte[] AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorValidateTime { get; set; }
    public bool AuthenticatorLoginEnable { get; set; }

    public bool IsTwoFactorAuthenticationEnable { get; set; }
    public bool IsActive { get; set; } = true;
}