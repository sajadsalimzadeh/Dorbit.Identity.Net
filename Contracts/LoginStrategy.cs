namespace Dorbit.Identity.Contracts;

public enum LoginStrategy
{
    None = 0,
    StaticPassword = 1,
    Cellphone = 2,
    Email = 3,
    Authenticator = 4
}