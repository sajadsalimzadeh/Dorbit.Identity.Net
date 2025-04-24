namespace Dorbit.Identity.Contracts.Otps;

public class OtpValidateRequest
{
    public string Receiver { get; set; }
    public string Code { get; set; }
}