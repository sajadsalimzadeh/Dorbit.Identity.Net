using System;
using Dorbit.Identity.Enums;

namespace Dorbit.Identity.Models.Auth;

public class AuthLoginWithOtpRequest
{
    public Guid Id { get; set; }
    public UserLoginStrategy LoginStrategy { get; set; }
    public string UserAgent { get; set; }
    public string Code { get; set; }
}