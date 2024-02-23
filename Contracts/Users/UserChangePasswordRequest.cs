using System;

namespace Dorbit.Identity.Contracts.Users;

public class UserChangePasswordRequest
{
    public LoginStrategy Strategy { get; set; }
    public Guid OtpId { get; set; }
    public string Value { get; set; }
    public string NewPassword { get; set; }
    public string RenewPassword { get; set; }
}