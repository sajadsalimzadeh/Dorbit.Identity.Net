using System.Security.Claims;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Models.Messages;
using Dorbit.Framework.Services;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Auth;
using Dorbit.Identity.Models.Otps;
using Dorbit.Identity.Models.Tokens;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class AuthService : IAuthService
{
    private readonly OtpService _otpService;
    private readonly UserRepository _userRepository;
    private readonly TokenService _tokenService;
    private readonly TokenRepository _tokenRepository;
    private readonly MessageManager _messageManager;
    private readonly PrivilegeService _privilegeService;
    private readonly IDistributedCache _distributedCache;

    public AuthService(
        OtpService otpService,
        TokenService tokenService,
        TokenRepository tokenRepository,
        UserRepository userRepository,
        MessageManager messageManager,
        PrivilegeService privilegeService,
        IDistributedCache distributedCache)
    {
        _otpService = otpService;
        _tokenService = tokenService;
        _tokenRepository = tokenRepository;
        _userRepository = userRepository;
        _messageManager = messageManager;
        _privilegeService = privilegeService;
        _distributedCache = distributedCache;
    }

    public async Task<AuthLoginResponse> Login(AuthLoginRequest request)
    {
        var user = _userRepository.Set().FirstOrDefault(x => x.Username == request.Username) ??
                   throw new OperationException(Errors.UsernameOrPasswordWrong);

        var isTwoFactorAuthenticationEnable = user.IsTwoFactorAuthenticationEnable;
        if (request.LoginStrategy == UserLoginStrategy.StaticPassword || request.LoginStrategy == UserLoginStrategy.None)
        {
            var hash = HashUtility.HashPassword(request.Value, user.Salt);
            if (user.PasswordHash != hash) throw new OperationException(Errors.UsernameOrPasswordWrong);
        }
        else
        {
            isTwoFactorAuthenticationEnable = true;
        }

        if (isTwoFactorAuthenticationEnable)
        {
            if (request.LoginStrategy == UserLoginStrategy.None)
            {
                if (user.CellphoneValidateTime.HasValue) request.LoginStrategy = UserLoginStrategy.Cellphone;
                else if (user.EmailValidateTime.HasValue) request.LoginStrategy = UserLoginStrategy.Email;
                else if (user.AuthenticatorValidateTime.HasValue) request.LoginStrategy = UserLoginStrategy.Authenticator;
            }
            else
            {
                if ((request.LoginStrategy == UserLoginStrategy.Cellphone && !user.CellphoneValidateTime.HasValue) ||
                    (request.LoginStrategy == UserLoginStrategy.Email && !user.EmailValidateTime.HasValue) ||
                    (request.LoginStrategy == UserLoginStrategy.Authenticator && !user.AuthenticatorValidateTime.HasValue))
                    throw new OperationException(Errors.LoginStrategyIsNotEnabled);
            }

            if (request.LoginStrategy == UserLoginStrategy.Cellphone || request.LoginStrategy == UserLoginStrategy.Email)
            {
                var otp = await SendOtp(new AuthSendOtpRequest()
                {
                    Value = user.Username,
                    LoginStrategy = request.LoginStrategy
                });

                return new AuthLoginResponse()
                {
                    OtpId = otp.Id,
                    UserLoginStrategy = request.LoginStrategy
                };
            }
        }

        return new AuthLoginResponse()
        {
            Token = await _tokenService.CreateAsync(new TokenNewRequest()
            {
                User = user,
                UserAgent = request.UserAgent,
            })
        };
    }

    public async Task<Otp> SendOtp(AuthSendOtpRequest request)
    {
        var otpLifetime = TimeSpan.FromSeconds(App.Setting.Security.OtpTimeoutInSec);
        var otp = await _otpService.CreateAsync(new OtpCreateRequest()
        {
            Duration = otpLifetime,
            Length = 5
        }, out var code);
        if (request.LoginStrategy == UserLoginStrategy.Cellphone)
        {
            await _messageManager.SendAsync(new MessageSmsRequest()
            {
                To = request.Value,
                TemplateId = App.Setting.Message.MeliPayamakOtpBodyId,
                Args = new object[] { code }
            });
        }
        else if (request.LoginStrategy == UserLoginStrategy.Email)
        {
            await _messageManager.SendAsync(new MessageEmailRequest()
            {
                To = request.Value,
                Subject = "Login one time password code",
                Body = "Code: {0}",
                Args = new object[] { otp.CodeHash }
            });
        }

        await _distributedCache.SetStringAsync(otp.Id.ToString(), request.Value,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = otpLifetime
            });

        return otp;
    }

    public async Task<AuthLoginResponse> CompleteLoginAsync(AuthLoginWithOtpRequest request)
    {
        var correlationId = request.Id.ToString();
        var otpValidateResult = await _otpService.ValidateAsync(new OtpValidateRequest()
        {
            Id = request.Id,
            Code = HashUtility.HashOtp(request.Code, correlationId)
        });
        if (!otpValidateResult) throw new OperationException(Errors.OtpValidateFailed);

        var userId = await _distributedCache.GetStringAsync(correlationId);
        if (userId is null) throw new OperationException(Errors.CorrelationIdIsExpired);

        if (!Guid.TryParse(userId, out var userGuid)) throw new OperationException(Errors.CorrelationIdIsInvalid);
        var user = await _userRepository.GetByIdAsync(userGuid);

        return new AuthLoginResponse()
        {
            Token = await _tokenService.CreateAsync(new TokenNewRequest()
            {
                User = user,
                UserAgent = request.UserAgent
            })
        };
    }

    public async Task<bool> IsTokenValid(HttpContext context, ClaimsPrincipal claimsPrincipal)
    {
        var tokenId = claimsPrincipal.FindFirst("Id")?.Value;
        if (!Guid.TryParse(tokenId, out var tokenGuid)) return false;
        if (!context.Request.Cookies.TryGetValue("CSRF", out var csrf)) return false;
        if (!Guid.TryParse(csrf, out var csrfGuid)) return false;
        if (csrfGuid != tokenGuid) return false;
        var token = await _tokenRepository.Set()
            .WithCacheAsync(set => set.FirstOrDefaultAsync(x => x.Id == tokenGuid), tokenId, TimeSpan.FromMinutes(1));
        if (token is null) return false;
        if (token.State != TokenState.Valid) return false;
        return true;
    }

    public async Task<bool> HasAccessAsync(Guid id, params string[] accesses)
    {
        var allUserAccess = await _privilegeService.GetAllByUserIdAsync(id);
        if (allUserAccess.Any(userAccess => accesses.Contains(userAccess))) return true;
        return false;
    }
}