using System.Text.Json.Serialization;

namespace Dorbit.Identity.Contracts.Auth;

public class AuthLoginWithAppleResponse
{
    [JsonPropertyName("code")]
    public string AuthenticationCode { get; set; }

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; }

    public string State { get; set; }
    public AuthLoginWithAppleResponseUserInfo User { get; set; }
}

public class AuthLoginWithAppleResponseUserInfo
{
    public AuthLoginWithAppleResponseUserName Name { get; set; }
    public string Email { get; set; }
}

public class AuthLoginWithAppleResponseUserName
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
}