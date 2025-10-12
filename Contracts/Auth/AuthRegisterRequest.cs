using System.ComponentModel.DataAnnotations;
using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthRegisterRequest
{
    [MaxLength(128), Required]
    public string Name { get; set; }
    [MaxLength(32), Required]
    public string Username { get; set; }
    [MaxLength(32)]
    public string Cellphone { get; set; }
    [MaxLength(64)]
    public string Email { get; set; }
    [MaxLength(32)]
    public string Password { get; set; }

    public OtpValidationRequest OtpValidation { get; set; }
}