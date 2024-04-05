using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Identity.Contracts;

namespace Dorbit.Identity.Databases.Entities;

public class Token : Entity
{
    public Guid UserId { get; set; }
    [MaxLength(128)]
    public string Os { get; set; }
    [MaxLength(128)]
    public string Platform { get; set; }
    [MaxLength(128)]
    public string Application { get; set; }
    public string Accesses { get; set; }
    public DateTime ExpireTime { get; set; }
    public TokenState State { get; set; } = TokenState.Valid;

    [ForeignKey(nameof(UserId))] public User User { get; set; }
}