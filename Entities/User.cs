using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Identities;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Utils.Json;
using Dorbit.Identity.Contracts.Users;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Table(nameof(User), Schema = "identity")]
[Index(nameof(Username), IsUnique = true)]
public class User : FullEntity
{
    [Sequence("seq_user_code", Schema = "identity")]
    public int Code { get; set; }

    [MaxLength(128)]
    public string Name { get; set; }

    [MaxLength(32), Required]
    public string Username { get; set; }

    [MaxLength(128), Required]
    public string PasswordSalt { get; set; }

    [MaxLength(128)]
    public string PasswordHash { get; set; }

    [MaxLength(20)]
    public string Cellphone { get; set; }
    [MaxLength(8)]
    public string CellphoneCountryCode { get; set; }
    public DateTime? CellphoneVerificationTime { get; set; }

    [MaxLength(64)]
    public string Email { get; set; }
    public DateTime? EmailVerificationTime { get; set; }

    [MaxLength(1024)]
    public string AuthenticatorKey { get; set; }
    public DateTime? AuthenticatorVerificationTime { get; set; }

    [MaxLength(512)]
    public string ThumbnailFilename { get; set; }

    public bool NeedResetPassword { get; set; }

    // Security
    public short MaxTokenCount { get; set; } = 1;
    public DateTime? LockoutEndTime { get; set; }
    public double Wallet { get; set; }

    [JsonField]
    public List<string> WhiteListIps { get; set; }

    [JsonField]
    public Dictionary<string, object> Infos { get; set; }
    
    [JsonField]
    public List<UserWebPushSubscription> WebPushSubscriptions { get; set; }
    
    [JsonField]
    public List<ClaimDto> Claims { get; set; }

    public bool IsTwoFactorAuthenticationEnabled { get; set; } = true;
    public UserStatus Status { get; set; } = UserStatus.Active;

    [MaxLength(1024)]
    public string Message { get; set; }
}