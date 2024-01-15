using System;

namespace Dorbit.Identity.Models.Otps;

public class OtpValidateRequest
{
    public Guid Id { get; set; }
    public string Code { get; set; }
}