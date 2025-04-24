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
    public async Task<AuthLoginResponse> LoginWithStaticPasswordAsync(AuthLoginWithStaticPasswordRequest request)
    {
        var username = request.Username.ToLower();
        var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Username == username) ??
                   throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

        var hash = HashUtility.HashPassword(request.Password, user.PasswordSalt);
        if (user.PasswordHash != hash) throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

        var csrfToken = Guid.NewGuid().ToString();
        return await tokenService.CreateAsync(new TokenCreateRequest()
        {
            User = user,
            CsrfToken = csrfToken,
            UserAgent = request.UserAgent,
        });
    }

    public async Task<AuthLoginResponse> LoginWithOtpAsync(AuthLoginWithOtpRequest request)
    {
        if (request.Type == OtpType.Cellphone)
        {
            var otpValidateResult = await otpService.ValidateAsync(new OtpValidateRequest()
            {
                Receiver = request.Receiver,
                Code = request.Code
            });
            if (!otpValidateResult) throw new OperationException(IdentityErrors.OtpValidateFailed);

            User user;
            if (request.Type == OtpType.Cellphone) user = await userRepository.GetByCellphoneAsync(request.Receiver);
            else if (request.Type == OtpType.Email) user = await userRepository.GetByEmailAsync(request.Receiver);
            else throw new OperationException(IdentityErrors.OtpTypeNotSupported);

            if (user is null)
            {
                throw new OperationException(IdentityErrors.UserNotExists);
            }

            var csrfToken = Guid.NewGuid().ToString();
            return await tokenService.CreateAsync(new TokenCreateRequest()
            {
                User = user,
                CsrfToken = csrfToken,
                UserAgent = request.UserAgent
            });
        }

        throw new OperationException(IdentityErrors.OtpTypeNotSupported);
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

        var csrfToken = Guid.NewGuid().ToString();
        return await tokenService.CreateAsync(new TokenCreateRequest()
        {
            User = user,
            CsrfToken = csrfToken
        });
    }

    public async Task<IUserDto> GetUserByTokenAsync(string token)
    {
        if (!jwtService.TryValidateToken(token, out _, out var claims)) throw new AuthenticationException("Token is invalid");

        if (!Guid.TryParse(claims.FindFirst(nameof(TokenClaimTypes.UserId))?.Value, out var userGuid)) 
            throw new Exception("UserId not correct format");
        
        var user = await userRepository.Set().GetByIdAsyncWithCache(userGuid, $"user-{userGuid}", TimeSpan.FromMinutes(1));
        if (user is null) return default;
        return new BaseUserDto()
        {
            Id = userGuid,
            Name = user.Name,
            Username = user.Username,
            IsActive = user.IsActive,
            Claims = claims,
        };
    }
    
    public async Task ChangePasswordByPasswordAsync(AuthChangePasswordByPasswordRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user.PasswordHash != HashUtility.HashPassword(request.Password, user.PasswordSalt))
            throw new OperationException(IdentityErrors.OldPasswordIsInvalid);
        
        user.PasswordSalt = Guid.NewGuid().ToString();
        user.PasswordHash = HashUtility.HashPassword(request.NewPassword, user.PasswordSalt);

        await userRepository.UpdateAsync(user);
    }

    public async Task ChangePasswordByOtpAsync(AuthChangePasswordByOtpRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);
        var validateResult = await otpService.ValidateAsync(new OtpValidateRequest()
        {
            Receiver = request.Receiver,
            Code = request.Code
        });
        if (!validateResult)
            throw new OperationException(IdentityErrors.OtpIsInvalid);
        
        user.PasswordSalt = Guid.NewGuid().ToString();
        user.PasswordHash = HashUtility.HashPassword(request.NewPassword, user.PasswordSalt);

        await userRepository.UpdateAsync(user);
    }

    public async Task<bool> IsTokenValid(HttpContext context, ClaimsPrincipal claimsPrincipal)
    {
        var tokenCsrf = claimsPrincipal.FindFirst(nameof(TokenClaimTypes.CsrfToken))?.Value;
        if (!context.Request.Cookies.TryGetValue(nameof(TokenClaimTypes.CsrfToken), out var userCsrf)) return false;
        
        if (userCsrf != tokenCsrf)
            throw new AuthenticationException("Csrf token not match");

        var tokenIdClaim = claimsPrincipal.FindFirst(nameof(TokenClaimTypes.Id));
        if(tokenIdClaim is null)
            throw new AuthenticationException("Token claim id not found");
        
        if (!Guid.TryParse(tokenIdClaim.Value, out var tokeGuid))
            throw new AuthenticationException("Token claim id invalid");
        
        var token = await tokenRepository.Set().FirstOrDefaultAsyncWithCache(x => x.Id == tokeGuid, tokeGuid.ToString(), TimeSpan.FromMinutes(2));
        if (token is null)
            throw new AuthenticationException("Token not found");
        
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

    public Task<bool> HasAccessAsync(string access)
    {
        return HasAccessAsync(userResolver.User.Id, access);
    }

    public async Task<bool> HasAccessAsync(object userId, string access)
    {
        var allUserAccess = await GetAllAccessAsync(userId);
        return allUserAccess.Any(access.Contains);
    }
}