using System;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Databases.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IUserResolver _userResolver;
    private readonly OtpService _otpService;

    public UserService(UserRepository userRepository, IUserResolver userResolver, OtpService otpService)
    {
        _userRepository = userRepository;
        _userResolver = userResolver;
        _otpService = otpService;
    }

    public Task<User> AddAsync(UserAddRequest request)
    {
        var entity = request.MapTo(new User()
        {
            Salt = Guid.NewGuid().ToString()
        });
        entity.PasswordHash = HashUtility.HashPassword(request.Password, entity.Salt);

        if ((request.ValidateTypes & UserValidateTypes.Cellphone) > 0 && !string.IsNullOrEmpty(request.Cellphone))
            entity.CellphoneValidateTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Email) > 0 && !string.IsNullOrEmpty(request.Email)) entity.EmailValidateTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Authenticator) > 0 && !string.IsNullOrEmpty(request.AuthenticatorKey))
            entity.AuthenticatorValidateTime = DateTime.Now;

        return _userRepository.InsertAsync(entity);
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
        return await _userRepository.RemoveAsync(id);
    }

    public async Task ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await _userRepository.Set().FirstOrDefaultAsync(x => x.Username == request.Username);
        await _userRepository.UpdateAsync(request.MapTo(user));
    }

    public async Task ChangePasswordAsync(UserChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(_userResolver.User.Id);

        if (request.NewPassword != request.RenewPassword)
            throw new OperationException(Errors.NewPasswordMissMach);


        if (request.NewPassword.Length < 4)
            throw new OperationException(Errors.NewPasswordIsWeak);

        switch (request.Strategy)
        {
            case LoginStrategy.StaticPassword:
            {
                if (user.PasswordHash != HashUtility.HashPassword(request.Value, user.Salt))
                    throw new OperationException(Errors.OldPasswordIsInvalid);
                break;
            }
            case LoginStrategy.Cellphone or LoginStrategy.Email:
            {
                var validateResult = await _otpService.ValidateAsync(new OtpValidateRequest()
                {
                    Id = request.OtpId,
                    Code = request.Value
                });
                if(!validateResult.Success)
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