using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Entities.Abstractions;
using Dorbit.Identity.Contracts.Otps;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Table(nameof(Otp), Schema = "identity")]
[Index(nameof(Receiver), IsUnique = true)]
public class Otp : Entity, ICreationTime
{
    [MaxLength(64)]
    public string Receiver { get; set; }
    public bool IsUsed { get; set; }
    public OtpType Type { get; set; }
    public byte TryRemain { get; set; }
    [MaxLength(64)] 
    public string Code { get; set; }
    public DateTime ExpireAt { get; set; }
    public DateTime CreationTime { get; set; }
}