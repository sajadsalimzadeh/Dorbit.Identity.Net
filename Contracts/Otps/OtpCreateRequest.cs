using System;

namespace Dorbit.Identity.Contracts.Otps;

public class OtpCreateRequest
{
    public string Receiver { get; set; }
    public byte Length { get; set; }
    public byte TryRemain { get; set; } = 3;
    public TimeSpan Duration { get; set; }
}