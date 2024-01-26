using System;
using System.ComponentModel.DataAnnotations;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Databases.Entities;

public class Otp : Entity, ICreationTime
{
    public bool IsUsed { get; set; }
    public byte TryRemain { get; set; }
    [StringLength(32)] public string CodeHash { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreationTime { get; set; }
}