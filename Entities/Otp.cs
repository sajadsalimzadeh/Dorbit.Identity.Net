using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Entities.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Table(nameof(Otp), Schema = "identity")]
[Index(nameof(CodeHash))]
public class Otp : Entity, ICreationTime
{
    public bool IsUsed { get; set; }
    public byte TryRemain { get; set; }
    [MaxLength(64)] public string CodeHash { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreationTime { get; set; }
}