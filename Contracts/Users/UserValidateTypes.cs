using System;

namespace Dorbit.Identity.Contracts.Users;

[Flags]
public enum UserValidateTypes
{
    Cellphone = 1,
    Email = 2,
    Authenticator = 4
}