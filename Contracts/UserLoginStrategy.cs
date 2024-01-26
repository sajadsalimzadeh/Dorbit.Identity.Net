namespace Dorbit.Identity.Contracts;

public enum UserLoginStrategy
{
    None = 0,
    StaticPassword = 1,
    Cellphone = 2,
    Email = 3,
    Authenticator = 4
}