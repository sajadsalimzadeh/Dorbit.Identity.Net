using System;
using System.Threading;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Options;
using Serilog;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class GoogleService(IOptions<ConfigGoogleOAuth> configGoogleOAuthOptions, ILogger logger)
{
    public async Task<Userinfo> ValidateAsync(AuthLoginWithGoogleRequest request)
    {
        logger.Information("Validating Google OAuth client ID {@Request}", request);
        var configGoogleOAuth = configGoogleOAuthOptions.Value;

        if (!configGoogleOAuth.ClientIds.TryGetValue(request.Platform, out var clientId))
            throw new Exception("Client Id not found");
        
        var initializer = new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = configGoogleOAuth.ClientSecret
            }
        };

        if (request.Verifier.IsNotNullOrEmpty())
        {
            logger.Information("PkceGoogleAuthorizationCodeFlow Initialize");
            var flow = new PkceGoogleAuthorizationCodeFlow(initializer);

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: request.Code,
                codeVerifier: request.Verifier,
                redirectUri: configGoogleOAuth.RedirectUrl,
                taskCancellationToken: CancellationToken.None
            );

            logger.Information("ExchangeCodeForTokenAsync");
            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var oauthService = new Oauth2Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DorbitApp"
            });

            logger.Information("GoogleCredential.FromAccessToken");
            return await oauthService.Userinfo.Get().ExecuteAsync();
        }
        else
        {
            logger.Information("GoogleAuthorizationCodeFlow Initialize");
            var flow = new GoogleAuthorizationCodeFlow(initializer);

            var tokenResponse = await flow.ExchangeCodeForTokenAsync(
                userId: "user",
                code: request.Code,
                redirectUri: configGoogleOAuth.RedirectUrl,
                taskCancellationToken: CancellationToken.None
            );

            logger.Information("ExchangeCodeForTokenAsync");
            var credential = GoogleCredential.FromAccessToken(tokenResponse.AccessToken);
            var oauthService = new Oauth2Service(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DorbitApp"
            });

            logger.Information("GoogleCredential.FromAccessToken");
            return await oauthService.Userinfo.Get().ExecuteAsync();
        }
    }
}