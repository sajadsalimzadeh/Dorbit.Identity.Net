using System;

namespace Dorbit.Identity.Contracts.Otps;

public class OtpValidateRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; }
}