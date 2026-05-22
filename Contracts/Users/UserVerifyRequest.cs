using System.Collections.Generic;
using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Users;

public class UserVerifyRequest : OtpValidationRequest
{
    public Dictionary<string, object> Infos { get; set; }
}