﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Abstractions;
using Dorbit.Framework.Contracts.Identities;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class IdentityService(
    OtpService otpService,
    JwtService jwtService,
    TokenService tokenService,
    UserService userService,
    RoleRepository roleRepository,
    UserRepository userRepository,
    TokenRepository tokenRepository,
    AccessRepository accessRepository,
    UserPrivilegeRepository userPrivilegeRepository,
    IOptions<ConfigIdentitySecurity> configIdentitySecurity
)
    : IIdentityService, IUserResolver
{
    public IdentityDto Identity { get; private set; }
    public IUserDto User { get; set; }

    private readonly ConfigIdentitySecurity _configIdentitySecurity = configIdentitySecurity.Value;

    public async Task<AuthLoginResponse> LoginWithPasswordAsync(AuthLoginWithPasswordRequest request)
    {
        var username = request.Username.ToLower();
        var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Username == username) ??
                   throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

        var hash = UserService.HashPassword(request.Password, user.PasswordSalt);
        if (user.PasswordHash != hash) throw new OperationException(IdentityErrors.UsernameOrPasswordWrong);

        return await tokenService.CreateAsync(new TokenCreateRequest()
        {
            User = user,
            CsrfToken = Guid.NewGuid().ToString(),
            UserAgent = request.UserAgent,
        });
    }

    public async Task<AuthLoginResponse> LoginWithGoogleAsync(AuthLoginWithGoogleRequest request)
    {
        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);
            var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Username == payload.Email);
            if (user is null)
            {
                user = await userService.AddAsync(new UserAddRequest()
                {
                    Username = payload.Email,
                    Name = payload.Name,
                    Email = payload.Email,
                });
            }

            return await tokenService.CreateAsync(new TokenCreateRequest()
            {
                User = user,
                CsrfToken = Guid.NewGuid().ToString(),
                UserAgent = request.UserAgent,
            });
        }
        catch (Exception ex)
        {
            throw new UnauthorizedAccessException("Authorization failed");
        }
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

    public async Task ChangePasswordByPasswordAsync(AuthChangePasswordByPasswordRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.UserId);

        if (user.PasswordHash != UserService.HashPassword(request.Password, user.PasswordSalt))
            throw new OperationException(IdentityErrors.OldPasswordIsInvalid);

        user.PasswordSalt = Guid.NewGuid().ToString();
        user.PasswordHash = UserService.HashPassword(request.NewPassword, user.PasswordSalt);

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
        user.PasswordHash = UserService.HashPassword(request.NewPassword, user.PasswordSalt);

        await userRepository.UpdateAsync(user);
    }

    public async Task<IdentityDto> ValidateAsync(IdentityValidateRequest request)
    {
        var secret = _configIdentitySecurity.Secret.GetDecryptedValue();
        if (request.AccessToken.IsNullOrEmpty())
            throw new AuthenticationException("Access token not set");

        if (!jwtService.TryValidateToken(request.AccessToken, secret, out _, out var claimsPrincipal))
            throw new AuthenticationException("Invalid access token");

        if (!claimsPrincipal.Claims.TryGetString(nameof(TokenClaimTypes.CsrfToken), out var csrfToken))
            throw new AuthenticationException("Csrf token not fount");

        if (!_configIdentitySecurity.IgnoreCsrfTokenValidation)
        {
            if (request.CsrfToken.IsNullOrEmpty())
                throw new AuthenticationException("Csrf token not set");

            if (request.CsrfToken != csrfToken)
                throw new AuthenticationException("Csrf token not match");
        }

        if (!claimsPrincipal.Claims.TryGetGuid(nameof(TokenClaimTypes.Id), out var tokenId))
            throw new AuthenticationException("Token claim id not found");

        var token = await tokenRepository.Set()
            .Include(x => x.User)
            .FirstOrDefaultAsyncWithCache(x => x.Id == tokenId, tokenId.ToString(), TimeSpan.FromMinutes(1));
        if (token is null)
            throw new AuthenticationException("Token not found");

        if (token.State != TokenState.Valid) return null;

        var now = DateTime.UtcNow;
        var userPrivileges = await userPrivilegeRepository.Set().Where(x =>
            x.UserId == token.UserId &&
            (x.From == null || x.From > now) &&
            (x.To == null || x.To < now)
        ).ToListAsyncWithCache($"Identity-{nameof(UserPrivilege)}-{token.UserId}", TimeSpan.FromMinutes(1));

        var roles = await roleRepository.Set()
            .ToListAsyncWithCache($"Identity-{nameof(Role)}-GetAll", TimeSpan.FromMinutes(5));

        Identity = new IdentityDto
        {
            User = User = token.User.MapTo<UserDto>()
        };
        var allAccessibility = new List<string>();
        foreach (var userPrivilege in userPrivileges)
        {
            if (userPrivilege.IsAdmin)
            {
                Identity.IsFullAccess = true;
                break;
            }

            var privilegeRoles = roles.Where(x => userPrivilege.RoleIds.Contains(x.Id)).ToList();
            allAccessibility.AddRange(privilegeRoles.Where(x => x.Accessibility != null)
                .SelectMany(x => x.Accessibility));
            if (userPrivilege.Accessibility is not null)
            {
                allAccessibility.AddRange(userPrivilege.Accessibility);
            }
        }

        Identity.Claims = token.User.Claims;
        Identity.Accessibility = allAccessibility;
        Identity.DeepAccessibility = await accessRepository.GetTotalAccessibilityAsync(allAccessibility);

        return Identity;
    }
}