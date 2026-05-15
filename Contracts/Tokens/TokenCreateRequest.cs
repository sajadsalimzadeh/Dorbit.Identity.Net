using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Contracts.Tokens;

public class TokenCreateRequest
{
    public required UserBase User { get; init; }
    public required string CsrfToken { get; init; }
    public string AccessToken { get; set; }
    public string UserAgent { get; set; }
}