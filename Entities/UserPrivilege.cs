using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Utils.Json;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Table(nameof(UserPrivilege), Schema = "identity")]
[Index(nameof(UserId), IsUnique = true)]
[Index(nameof(TenantId), IsUnique = true)]
public class UserPrivilege : FullEntity
{
    public Guid UserId { get; set; }
    public Guid? TenantId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool IsFullAccess { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    [JsonField]
    public List<Guid> RoleIds { get; set; }
    
    [JsonField]
    public List<string> Accessibility { get; set; }
}