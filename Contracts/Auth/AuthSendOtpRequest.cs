namespace Dorbit.Identity.Contracts.Auth;

public class AuthSendOtpRequest
{
    public string Value { get; set; }
    public AuthMethod Method { get; set; }
}