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
public class UserService(
    OtpService otpService,
    IUserResolver userResolver,
    UserRepository userRepository,
    TokenRepository tokenRepository,
    UserPrivilegeRepository userPrivilegeRepository,
    IOptions<ConfigIdentitySecurity> configSecurityOptions)
{
    private readonly ConfigIdentitySecurity _configIdentitySecurity = configSecurityOptions.Value;

    public async Task<User> AddAsync(UserAddRequest request)
    {
        var existsUser = await userRepository.Set(false).FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username);
        if (existsUser is not null && !existsUser.IsDeleted) throw new OperationException(IdentityErrors.UserExists);
        var entity = request.MapTo(existsUser ?? new User()
        {
            PasswordSalt = Guid.NewGuid().ToString()
        });
        entity.Username = entity.Username.ToLower();
        entity.PasswordHash = HashUtility.HashPassword(request.Password, entity.PasswordSalt);

        if ((request.ValidateTypes & UserValidateTypes.Cellphone) > 0 && !string.IsNullOrEmpty(request.Cellphone))
            entity.PhoneNumberConfirmTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Email) > 0 && !string.IsNullOrEmpty(request.Email)) entity.EmailConfirmTime = DateTime.Now;
        if ((request.ValidateTypes & UserValidateTypes.Authenticator) > 0 && !string.IsNullOrEmpty(request.AuthenticatorKey))
            entity.AuthenticatorValidateTime = DateTime.Now;

        entity.IsDeleted = false;
        return await userRepository.SaveAsync(entity);
    }

    public async Task<User> EditAsync(UserEditRequest request)
    {
        var entity = await userRepository.GetByIdAsync(request.Id);
        return await userRepository.UpdateAsync(request.MapTo(entity));
    }

    public async Task<User> RemoveAsync(Guid id)
    {
        var admin = await userRepository.GetAdminAsync();
        if (admin.Id == id) throw new OperationException(IdentityErrors.CanNotRemoveAdminUser);
        var transaction = userRepository.DbContext.BeginTransaction();
        await userPrivilegeRepository.BulkDeleteAsync(x => x.UserId == id);
        await tokenRepository.BulkDeleteAsync(x => x.UserId == id);
        var dto = await userRepository.DeleteAsync(id);
        await transaction.CommitAsync();
        return dto;
    }

    public async Task<User> ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Id == request.Id);
        user.PasswordHash = HashUtility.HashPassword(request.Password, user.PasswordSalt);
        await userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<User> DeActiveAsync(UserDeActiveRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        var admin = await userRepository.GetAdminAsync();
        if (admin.Id == user.Id) throw new OperationException(IdentityErrors.CanNotDeActiveAdmin);
        user.IsActive = false;
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }

    public async Task<User> ActiveAsync(UserActiveRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        user.IsActive = true;
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }

    public async Task<User> SetMessageAsync(UserMessageRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }
}