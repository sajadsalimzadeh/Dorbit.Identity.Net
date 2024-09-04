using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly TokenRepository _tokenRepository;
    private readonly PrivilegeRepository _privilegeRepository;
    private readonly IUserResolver _userResolver;
    private readonly OtpService _otpService;
    private readonly ConfigIdentitySecurity _configIdentitySecurity;

    public UserService(
        OtpService otpService,
        IUserResolver userResolver,
        UserRepository userRepository,
        TokenRepository tokenRepository,
        PrivilegeRepository privilegeRepository,
        IOptions<ConfigIdentitySecurity> configSecurityOptions
    )
    {
        _userRepository = userRepository;
        _tokenRepository = tokenRepository;
        _privilegeRepository = privilegeRepository;
        _userResolver = userResolver;
        _otpService = otpService;
        _configIdentitySecurity = configSecurityOptions.Value;
    }

    public async Task<User> AddAsync(UserAddRequest request)
    {
        var existsUser = await _userRepository.Set(false).FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username);
        if (existsUser is not null && !existsUser.IsDeleted) throw new OperationException(Errors.UserExists);
        var entity = request.MapTo(existsUser ?? new User()
        {
            Salt = Guid.NewGuid().ToString()
        });
        entity.Username = entity.Username.ToLower();
        entity.PasswordHash = HashUtility.HashPassword(request.Password, entity.Salt);

        if ((request.ValidateTypes & UserValidateTypes.Cellphone) > 0 && !string.IsNullOrEmpty(request.Cellphone))
            entity.CellphoneValidateTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Email) > 0 && !string.IsNullOrEmpty(request.Email)) entity.EmailValidateTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Authenticator) > 0 && !string.IsNullOrEmpty(request.AuthenticatorKey))
            entity.AuthenticatorValidateTime = DateTime.Now;

        entity.IsDeleted = false;
        return await _userRepository.SaveAsync(entity);
    }

    public async Task<User> EditAsync(UserEditRequest request)
    {
        var entity = await _userRepository.GetByIdAsync(request.Id);
        return await _userRepository.UpdateAsync(request.MapTo(entity));
    }

    public async Task<User> RemoveAsync(Guid id)
    {
        var admin = await _userRepository.GetAdminAsync();
        if (admin.Id == id) throw new OperationException(Errors.CanNotRemoveAdminUser);
        var transaction = _userRepository.DbContext.BeginTransaction();
        await _privilegeRepository.BulkDeleteAsync(x => x.UserId == id);
        await _tokenRepository.BulkDeleteAsync(x => x.UserId == id);
        var dto = await _userRepository.DeleteAsync(id);
        await transaction.CommitAsync();
        return dto;
    }

    public async Task<User> ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await _userRepository.Set().FirstOrDefaultAsync(x => x.Id == request.Id);
        user.PasswordHash = HashUtility.HashPassword(request.Password, user.Salt);
        await _userRepository.UpdateAsync(user);
        return user;
    }

    public async Task ChangePasswordAsync(UserChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync((Guid)_userResolver.User.Id);

        if (request.NewPassword != request.RenewPassword)
            throw new OperationException(Errors.NewPasswordMissMach);


        if (!new Regex(_configIdentitySecurity.PasswordPattern).IsMatch(request.NewPassword))
            throw new OperationException(Errors.NewPasswordIsWeak);

        switch (request.Method)
        {
            case AuthMethod.StaticPassword:
            {
                if (user.PasswordHash != HashUtility.HashPassword(request.Value, user.Salt))
                    throw new OperationException(Errors.OldPasswordIsInvalid);
                break;
            }
            case AuthMethod.Cellphone or AuthMethod.Email:
            {
                var validateResult = await _otpService.ValidateAsync(new OtpValidateRequest()
                {
                    Id = request.OtpId,
                    Code = request.Value
                });
                if (!validateResult.Success)
                    throw new OperationException(Errors.OtpIsInvalid);
                break;
            }
            default:
                throw new OperationException(Errors.LoginStrategyNotSupported);
        }

        user.Salt = Guid.NewGuid().ToString();
        user.PasswordHash = HashUtility.HashPassword(request.NewPassword, user.Salt);

        await _userRepository.UpdateAsync(user);
    }

    public async Task<User> DeActiveAsync(UserDeActiveRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        var admin = await _userRepository.GetAdminAsync();
        if (admin.Id == user.Id) throw new OperationException(Errors.CanNotDeActiveAdmin);
        user.IsActive = false;
        user.Message = request.Message;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> ActiveAsync(UserActiveRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        user.IsActive = true;
        user.Message = request.Message;
        return await _userRepository.UpdateAsync(user);
    }

    public async Task<User> SetMessageAsync(UserMessageRequest request)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);
        user.Message = request.Message;
        return await _userRepository.UpdateAsync(user);
    }
}