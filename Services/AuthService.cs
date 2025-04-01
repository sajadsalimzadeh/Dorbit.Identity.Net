using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Abstractions;
using Dorbit.Framework.Contracts.Users;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
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
public class AuthService(
    OtpService otpService,
    JwtService jwtService,
    TokenService tokenService,
    IUserResolver userResolver,
    UserService userService,
    UserRepository userRepository,
    TokenRepository tokenRepository,
    PrivilegeService privilegeService)
    : IAuthService
{
    public async Task<AuthLoginResponse> LoginAsync(AuthLoginRequest request)
    {
        var username = request.Username.ToLower();
        if (request.LoginStrategy == AuthMethod.StaticPassword)
        {
            var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Username == username) ??
                       throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

            var hash = HashUtility.HashPassword(request.Value, user.Salt);
            if (user.PasswordHash != hash) throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

            return new AuthLoginResponse()
            {
                Token = await tokenService.CreateAsync(new TokenNewRequest()
                {
                    User = user,
                    UserAgent = request.UserAgent,
                })
            };
        }

        if (request.LoginStrategy == AuthMethod.Cellphone || request.LoginStrategy == AuthMethod.Email)
        {
            var otp = await otpService.SendOtp(new AuthSendOtpRequest()
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
            var otpValidateResult = await otpService.ValidateAsync(new OtpValidateRequest()
            {
                Id = request.OtpId,
                Code = request.Code
            });
            if (!otpValidateResult.Success) throw new OperationException(IdentityErrors.OtpValidateFailed);

            User user;
            if (request.LoginStrategy == AuthMethod.Cellphone) user = await userRepository.GetByCellphoneAsync(otpValidateResult.Value);
            else if (request.LoginStrategy == AuthMethod.Email) user = await userRepository.GetByEmailAsync(otpValidateResult.Value);
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

                user = await userService.AddAsync(userAddRequest);
            }

            return new AuthLoginResponse()
            {
                Token = await tokenService.CreateAsync(new TokenNewRequest()
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
        var user = await userRepository.FirstOrDefaultAsync(x => x.Username == request.Username);
        if (user is not null) throw new OperationException(IdentityErrors.UserExists);

        user = await userService.AddAsync(new UserAddRequest()
        {
            Name = request.Name,
            Email = request.Email,
            Username = request.Username,
            Password = request.Password,
        });

        return new AuthLoginResponse()
        {
            Token = await tokenService.CreateAsync(new TokenNewRequest()
            {
                User = user,
            })
        };
    }

    public async Task<IUserDto> GetUserByTokenAsync(string token)
    {
        if (!await jwtService.TryValidateTokenAsync(token, out _, out var claims)) throw new AuthenticationException("Token is invalid");
        
        var id = claims.FindFirst("UserId")?.Value ?? claims.FindFirst("Id")?.Value;
        if(!Guid.TryParse(claims.FindFirst("UserId")?.Value, out var userGuid)) throw new Exception("UserId not correct format");
        var user = await userRepository.Set().GetByIdAsyncWithCache(userGuid, $"user-{userGuid}", TimeSpan.FromMinutes(1));
        if (user is null) return default;
        return new BaseUserDto()
        {
            Id = Guid.Parse(id ?? ""),
            Name = user.Name,
            Username = user.Username,
            IsActive = user.IsActive,
            Claims = claims,
        };
    }

    public async Task<bool> IsTokenValid(HttpContext context, ClaimsPrincipal claimsPrincipal)
    {
        var tokenId = claimsPrincipal.FindFirst("Id")?.Value;
        if (!Guid.TryParse(tokenId, out var tokenGuid)) return false;
        if (!context.Request.Cookies.TryGetValue("CSRF", out var csrf)) return false;
        if (!Guid.TryParse(csrf, out var csrfGuid)) return false;
        if (csrfGuid != tokenGuid) return false;
        var token = await tokenRepository.Set()
            .WithCacheAsync(set => set.FirstOrDefaultAsync(x => x.Id == tokenGuid), tokenId, TimeSpan.FromMinutes(1));
        if (token is null) return false;
        if (token.State != TokenState.Valid) return false;
        return true;
    }

    public async Task<IEnumerable<string>> GetAllAccessAsync()
    {
        if (userResolver.User is null) return [];
        return await GetAllAccessAsync(userResolver.User.Id);
    }

    public Task<IEnumerable<string>> GetAllAccessAsync(object userId)
    {
        return privilegeService.GetAllByUserIdAsync(userId);
    }

    public Task<bool> HasAccessAsync(params string[] accesses)
    {
        return HasAccessAsync(userResolver.User.Id, accesses);
    }

    public async Task<bool> HasAccessAsync(object userId, params string[] accesses)
    {
        var allUserAccess = await GetAllAccessAsync(userId);
        return allUserAccess.Any(accesses.Select(x => x.ToLower()).Contains);
    }
}