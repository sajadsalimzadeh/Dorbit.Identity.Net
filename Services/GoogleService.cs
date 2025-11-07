using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class GoogleService(IOptions<ConfigGoogleOAuth> configGoogleOAuthOptions)
{
    public async Task<Userinfo> ValidateAsync(AuthLoginWithGoogleRequest request)
    {
        
        var configGoogleOAuth = configGoogleOAuthOptions.Value;
        var initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = configGoogleOAuth.ClientId,
                ClientSecret = configGoogleOAuth.ClientSecret
            }
        };

        var flow = new GoogleAuthorizationCodeFlow(initializer);

        var tokenResponse = await flow.ExchangeCodeForTokenAsync(
            userId: "user",
            code: request.AuthenticationCode,
            redirectUri: configGoogleOAuth.RedirectUrl,
            taskCancellationToken: CancellationToken.None
        );

        var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
        var oauthService = new Oauth2Service(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyApp"
        });
        
        
        return await oauthService.Userinfo.Get().ExecuteAsync();
    }
}