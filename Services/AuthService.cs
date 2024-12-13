using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class AuthService : IAuthService
{
    private readonly OtpService _otpService;
    private readonly TokenService _tokenService;
    private readonly IUserResolver _userResolver;
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly TokenRepository _tokenRepository;
    private readonly PrivilegeService _privilegeService;

    public AuthService(
        OtpService otpService,
        TokenService tokenService,
        IUserResolver userResolver,
        UserService userService,
        UserRepository userRepository,
        TokenRepository tokenRepository,
        PrivilegeService privilegeService)
    {
        _otpService = otpService;
        _tokenService = tokenService;
        _userResolver = userResolver;
        _userService = userService;
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _privilegeService = privilegeService;
    }

    public async Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request)
    {
        var username = request.Username.ToLower();
        if (request.LoginStrategy == AuthMethod.StaticPassword)
        {
            var user = await _userRepository.Set().FirstOrDefaultAsync(x => x.Username == username) ??
                       throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

            var hash = HashUtility.HashPassword(request.Value, user.Salt);
            if (user.PasswordHash != hash) throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

            return new AuthLoginResponse()
            {
                Token = await _tokenService.CreateAsync(new TokenNewRequest()
                {
                    User = user,
                    UserAgent = request.UserAgent,
                })
            };
        }

        if (request.LoginStrategy == AuthMethod.Cellphone || request.LoginStrategy == AuthMethod.Email)
        {
            var otp = await _otpService.SendOtp(new AuthSendOtpRequest()
            {
                Value = username,
                Method = request.LoginStrategy
            });

            return new AuthLoginResponse()
            {
                OtpId = otp.Id,
                LoginStrategy = request.LoginStrategy
            };
        }

        if (request.LoginStrategy == AuthMethod.Authenticator)
        {
        }

        throw new OperationException(IdentityErrors.LoginStrategyNotSupported);
    }

    public async Task<AuthLoginResponse> LoginWithCodeAsync(AuthLoginWithCodeRequest request)
    {
        if (request.LoginStrategy == AuthMethod.Cellphone || request.LoginStrategy == AuthMethod.Email)
        {
            var otpValidateResult = await _otpService.ValidateAsync(new OtpValidateRequest()
            {
                Id = request.OtpId,
                Code = request.Code
            });
            if (!otpValidateResult.Success) throw new OperationException(IdentityErrors.OtpValidateFailed);

            User user;
            if (request.LoginStrategy == AuthMethod.Cellphone) user = await _userRepository.GetByCellphoneAsync(otpValidateResult.Value);
            else if (request.LoginStrategy == AuthMethod.Email) user = await _userRepository.GetByEmailAsync(otpValidateResult.Value);
            else throw new OperationException(IdentityErrors.LoginStrategyNotSupported);

            if (user is null)
            {
                throw new OperationException(IdentityErrors.UserNotExists);
                var userAddRequest = new UserAddRequest();

                if (request.LoginStrategy == AuthMethod.Cellphone)
                {
                    userAddRequest.Username = userAddRequest.Cellphone = otpValidateResult.Value;
                    userAddRequest.ValidateTypes = UserValidateTypes.Cellphone;
                }
                else if (request.LoginStrategy == AuthMethod.Email)
                {
                    userAddRequest.Username = userAddRequest.Email = otpValidateResult.Value;
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

        throw new OperationException(IdentityErrors.LoginStrategyNotSupported);
    }

    public async Task<AuthLoginResponse> RegisterAsync(AuthRegisterRequest request)
    {
        var user = await _userRepository.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (user is not null) throw new OperationException(IdentityErrors.UserExists);

        user = await _userService.AddAsync(new UserAddRequest()
        {
            Name = request.Name,
            Email = request.Email,
            Username = request.Username,
            Password = request.Password,
        });
        
        return new AuthLoginResponse()
        {
            Token = await _tokenService.CreateAsync(new TokenNewRequest()
            {
                User = user,
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

    public Task<bool> HasAccessAsync(params string[] accesses)
    {
        return HasAccessAsync(_userResolver.User.Id, accesses);
    }

    public async Task<bool> HasAccessAsync(object id, params string[] accesses)
    {
        var allUserAccess = await _privilegeService.GetAllByUserIdAsync(id);
        return allUserAccess.Any(accesses.Select(x => x.ToLower()).Contains);
    }
}