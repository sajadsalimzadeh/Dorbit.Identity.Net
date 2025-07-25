using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Utils.Json;
using Dorbit.Identity.Contracts.Users;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace Dorbit.Identity.Entities;

[Table(nameof(User), Schema = "identity")]
[Index(nameof(Username), IsUnique = true)]
public class User : FullEntity
{
    [Sequence("seq_user_code", Schema = "identity")]
    public int Code { get; set; }

    [StringLength(128)]
    public string Name { get; set; }

    [StringLength(32), Required]
    public string Username { get; set; }

    [StringLength(128), Required]
    public string PasswordSalt { get; set; }

    [StringLength(128)]
    public string PasswordHash { get; set; }

    [StringLength(20)]
    public string Cellphone { get; set; }

    public DateTime? CellphoneConfirmTime { get; set; }

    [StringLength(64)]
    public string Email { get; set; }

    public DateTime? EmailConfirmTime { get; set; }

    [StringLength(1024)]
    public string AuthenticatorKey { get; set; }

    public DateTime? AuthenticatorValidateTime { get; set; }

    [StringLength(512)]
    public string ThumbnailFilename { get; set; }

    public bool NeedResetPassword { get; set; }

    // Security
    public short MaxTokenCount { get; set; } = 1;
    public DateTime? LockoutEndTime { get; set; }
    public double Wallet { get; set; }

    [JsonField]
    public List<string> WhiteListIps { get; set; }
    
    [JsonField]
    public List<UserWebPushSubscription> WebPushSubscriptions { get; set; }
    
    [JsonField]
    public Dictionary<string, string> Claims { get; set; }

    public bool IsTwoFactorAuthenticationEnabled { get; set; } = true;
    public UserStatus Status { get; set; } = UserStatus.Active;

    [MaxLength(1024)]
    public string Message { get; set; }
}