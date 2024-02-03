using System;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Messages;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Databases.Entities;
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
    private readonly TokenService _tokenService;
    private readonly IUserResolver _userResolver;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly MessageManager _messageManager;
    private readonly TokenRepository _tokenRepository;
    private readonly PrivilegeService _privilegeService;
    private readonly IDistributedCache _distributedCache;

    public AuthService(OtpService otpService,
        TokenService tokenService,
        IUserResolver userResolver,
        UserService userService,
        UserRepository userRepository,
        MessageManager messageManager,
        TokenRepository tokenRepository,
        PrivilegeService privilegeService,
        IDistributedCache distributedCache)
    {
        _otpService = otpService;
        _tokenService = tokenService;
        _userResolver = userResolver;
        _userService = userService;
        _userRepository = userRepository;
        _messageManager = messageManager;
        _tokenRepository = tokenRepository;
        _privilegeService = privilegeService;
        _distributedCache = distributedCache;
    }

    public async Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request)
    {
        if (request.LoginStrategy == LoginStrategy.StaticPassword)
        {
            var user = _userRepository.Set().FirstOrDefault(x => x.Username == request.Username) ??
                       throw new OperationException(Errors.UsernameOrPasswordWrong);

            var hash = HashUtility.HashPassword(request.Value, user.Salt);
            if (user.PasswordHash != hash) throw new OperationException(Errors.UsernameOrPasswordWrong);

            return new AuthLoginResponse()
            {
                Token = await _tokenService.CreateAsync(new TokenNewRequest()
                {
                    User = user,
                    UserAgent = request.UserAgent,
                })
            };
        }

        if (request.LoginStrategy == LoginStrategy.Cellphone || request.LoginStrategy == LoginStrategy.Email)
        {
            var otp = await SendOtp(new AuthSendOtpRequest()
            {
                Value = request.Username,
                LoginStrategy = request.LoginStrategy
            });

            return new AuthLoginResponse()
            {
                OtpId = otp.Id,
                LoginStrategy = request.LoginStrategy
            };
        }

        if (request.LoginStrategy == LoginStrategy.Authenticator)
        {
        }

        throw new OperationException(Errors.LoginStrategyNotSupported);
    }

    public async Task<Otp> SendOtp(AuthSendOtpRequest request)
    {
        var otpLifetime = TimeSpan.FromSeconds(AppIdentity.Setting.Security.OtpTimeoutInSec);
        var otp = await _otpService.CreateAsync(new OtpCreateRequest()
        {
            Duration = otpLifetime,
            Length = 5
        }, out var code);
        if (request.LoginStrategy == LoginStrategy.Cellphone)
        {
            await _messageManager.SendAsync(new MessageSmsRequest()
            {
                To = request.Value,
                TemplateType = MessageTemplateType.Otp,
                Args = [code]
            });
        }
        else if (request.LoginStrategy == LoginStrategy.Email)
        {
            await _messageManager.SendAsync(new MessageEmailRequest()
            {
                To = request.Value,
                Subject = "Login one time password code",
                Body = "Code: {0}",
                Args = [code]
            });
        }

        await _distributedCache.SetStringAsync(otp.Id.ToString(), request.Value,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = otpLifetime
            });

        return otp;
    }

    public async Task<AuthLoginResponse> LoginWithCodeAsync(AuthLoginWithCodeRequest request)
    {
        var value = await _distributedCache.GetStringAsync(request.OtpId.ToString());
        if (value is null) throw new OperationException(Errors.CorrelationIdIsExpired);

        if (request.LoginStrategy == LoginStrategy.Cellphone || request.LoginStrategy == LoginStrategy.Email)
        {
            var otpValidateResult = await _otpService.ValidateAsync(new OtpValidateRequest()
            {
                Id = request.OtpId,
                Code = request.Code
            });
            if (!otpValidateResult) throw new OperationException(Errors.OtpValidateFailed);

            User user;
            if (request.LoginStrategy == LoginStrategy.Cellphone) user = await _userRepository.GetByCellphoneAsync(value);
            else if (request.LoginStrategy == LoginStrategy.Email) user = await _userRepository.GetByEmailAsync(value);
            else throw new OperationException(Errors.LoginStrategyNotSupported);

            if (user is null)
            {
                var userAddRequest = new UserAddRequest();

                if (request.LoginStrategy == LoginStrategy.Cellphone)
                {
                    userAddRequest.Username = userAddRequest.Cellphone = value;
                    userAddRequest.ValidateTypes = UserValidateTypes.Cellphone;
                }
                else if (request.LoginStrategy == LoginStrategy.Email)
                {
                    userAddRequest.Username = userAddRequest.Email = value;
                    userAddRequest.ValidateTypes = UserValidateTypes.Email;
                }

                user = await _userService.AddAsync(userAddRequest);
            }

            return new AuthLoginResponse()
            {
                Token = await _tokenService.CreateAsync(new TokenNewRequest()
                {
                    User = user,
                    UserAgent = request.UserAgent
                })
            };
        }

        throw new OperationException(Errors.LoginStrategyNotSupported);
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

    public Task<bool> HasAccessAsync(params string[] accesses)
    {
        return HasAccessAsync(_userResolver.User.Id, accesses);
    }

    public async Task<bool> HasAccessAsync(Guid id, params string[] accesses)
    {
        var allUserAccess = await _privilegeService.GetAllByUserIdAsync(id);
        return allUserAccess.Any(accesses.Select(x => x.ToLower()).Contains);
    }
}