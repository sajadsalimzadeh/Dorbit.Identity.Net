using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Utilities;
using Microsoft.Extensions.Options;
using Serilog;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class AppleService(IOptions<ConfigAppleOAuth> configAppleOAuthOptions, ILogger logger)
{
    public async Task<AuthLoginWithAppleResponse> ValidateAsync(AuthLoginWithAppleRequest request)
    {
        var configAppleOAuth = configAppleOAuthOptions.Value;
        var form = new Dictionary<string, string>
        {
            { "client_id", configAppleOAuth.ClientId },
            { "client_secret", AppleUtil.GenerateClientSecret(configAppleOAuth.TeamId, configAppleOAuth.ClientId, configAppleOAuth.KeyId, configAppleOAuth.PrivateKey) },
            { "code", request.AuthenticationCode },
            { "grant_type", "authorization_code" },
            { "redirect_uri", configAppleOAuth.RedirectUrl }
        };

        var httpClient = new HttpClient();
        var response = await httpClient.PostAsync("https://appleid.apple.com/auth/token", new FormUrlEncodedContent(form));
        var content = await response.Content.ReadAsStringAsync();

        logger.Information("Sign in with apple token info: {@content}", content);
        var result = JsonSerializer.Deserialize<AuthLoginWithAppleResponse>(content, JsonSerializerOptions.Web);

        if (result.IdToken.IsNotNullOrEmpty() && result.User == null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.IdToken);
            result.User = new AuthLoginWithAppleResponseUserInfo()
            {
                Email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value
            };
        }

        return result;
    }
}