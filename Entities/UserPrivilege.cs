using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Innofactor.EfCoreJsonValueConverter;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Table(nameof(UserPrivilege), Schema = "identity")]
[Index(nameof(UserId), IsUnique = true)]
public class UserPrivilege : FullEntity
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    [JsonField]
    public List<string> Roles { get; set; }
    
    [JsonField]
    public List<string> Accesses { get; set; }
}