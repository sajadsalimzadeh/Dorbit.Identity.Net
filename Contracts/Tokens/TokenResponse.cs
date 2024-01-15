using System;

namespace Dorbit.Identity.Models.Tokens;

public class TokenResponse
{
    public string Key { get; set; }
    public Guid Csrf { get; set; }
}