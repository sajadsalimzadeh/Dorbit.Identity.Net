using System;

namespace Dorbit.Identity.Models.Otps;

public class OtpCreateRequest
{
    public byte Length { get; set; }
    public byte TryRemain { get; set; } = 3;
    public TimeSpan Duration { get; set; }
}