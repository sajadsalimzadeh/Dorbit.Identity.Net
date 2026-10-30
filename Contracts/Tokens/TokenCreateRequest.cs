using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Contracts.Tokens;

public class TokenCreateRequest
{
    public required UserBase User { get; init; }
    public string CsrfToken { get; set; }
    public string AccessToken { get; set; }
    public string UserAgent { get; set; }
    public bool IsApiToken { get; set; }
    public long? TimeoutInSecond { get; set; }
}