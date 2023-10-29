using System.ComponentModel.DataAnnotations;
using Dorbit.Entities;
using Dorbit.Entities.Abstractions;

namespace Dorbit.Identity.Entities;

public class Otp : Entity, ICreationTime
{
    public bool IsUsed { get; set; }
    public byte TryRemain { get; set; }
    [StringLength(32)] public string Code { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreationTime { get; set; }
}