using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Notifications;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Databases.Abstractions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserBaseService(
    OtpService otpService,
    UserBaseRepository userBaseRepository,
    TokenRepository tokenRepository,
    IIdentityDbContext identityDbContext,
    TranslationService translationService,
    NotificationService notificationService,
    UserPrivilegeRepository userPrivilegeRepository,
    IUserServiceWrapper userServiceWrapper = null)
{
    public async Task<UserBase> AddAsync(UserAddRequest request)
    {
        userServiceWrapper?.OnAddExecutingAsync(request).Wait();
        var existsUser = await userBaseRepository.GetByUsernameAsync(request.Username);
        if (existsUser is not null && !existsUser.IsDeleted) throw new OperationException(IdentityErrors.UserExists);
        var entity = request.MapTo(existsUser ?? identityDbContext.CreateNewUser());
        entity.PasswordSalt = Guid.NewGuid().ToString();
        entity.Username = entity.Username.ToLower();
        request.Password ??= new Random().NextString(12);
        entity.PasswordHash = HashUtil.PasswordV2(request.Password, entity.PasswordSalt);

        if ((request.ValidateTypes & UserValidateTypes.Cellphone) > 0 && !string.IsNullOrEmpty(request.Cellphone))
            entity.CellphoneVerificationTime = DateTime.UtcNow;

        if ((request.ValidateTypes & UserValidateTypes.Email) > 0 && !string.IsNullOrEmpty(request.Email))
            entity.EmailVerificationTime = DateTime.UtcNow;

        if ((request.ValidateTypes & UserValidateTypes.Authenticator) > 0 && !string.IsNullOrEmpty(request.AuthenticatorKey))
            entity.AuthenticatorVerificationTime = DateTime.UtcNow;

        entity.IsDeleted = false;
        return await userBaseRepository.SaveAsync(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var admin = await userBaseRepository.GetAdminAsync();
        if (admin.Id == id) throw new OperationException(IdentityErrors.CanNotRemoveAdminUser);
        var transaction = userBaseRepository.DbContext.BeginTransaction();
        await userPrivilegeRepository.BulkDeleteAsync(x => x.UserId == id);
        await tokenRepository.BulkDeleteAsync(x => x.UserId == id);
        await userBaseRepository.DeleteAsync(id);
        await transaction.CommitAsync();
    }

    public async Task<UserBase> ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await userBaseRepository.Set().FirstOrDefaultAsync(x => x.Id == request.Id);
        user.PasswordHash = HashUtil.PasswordV2(request.Password, user.PasswordSalt);
        await userBaseRepository.UpdateAsync(user);
        return user;
    }

    public async Task<UserBase> InActiveAsync(UserDeActiveRequest request)
    {
        var user = await userBaseRepository.GetByIdAsync(request.Id);
        var admin = await userBaseRepository.GetAdminAsync();
        if (admin.Id == user.Id) throw new OperationException(IdentityErrors.CanNotDeActiveAdmin);
        user.Status = UserStatus.InActive;
        user.Message = request.Message;
        return await userBaseRepository.UpdateAsync(user);
    }

    public async Task<UserBase> ActiveAsync(UserActiveRequest request)
    {
        var user = await userBaseRepository.GetByIdAsync(request.Id);
        user.Status = UserStatus.Active;
        user.Message = request.Message;
        return await userBaseRepository.UpdateAsync(user);
    }

    public async Task<UserBase> SetMessageAsync(UserMessageRequest request)
    {
        var user = await userBaseRepository.GetByIdAsync(request.Id);
        user.Message = request.Message;
        return await userBaseRepository.UpdateAsync(user);
    }

    public async Task PushNotificationAsync(List<Guid> userIds, NotificationRequest request, Dictionary<string, string> translationArguments = null)
    {
        var users = await userBaseRepository.Set().Where(x => userIds.Contains(x.Id)).Select(x => new UserBase()
        {
            NotifySubscriptions = x.NotifySubscriptions
        }).ToListAsync();
        await PushNotificationAsync(users, request, translationArguments);
    }

    public Task PushNotificationAsync(List<UserBase> users, NotificationRequest request, Dictionary<string, string> translationArguments = null)
    {
        foreach (var user in users.Where(x => x.NotifySubscriptions != null))
        {
            foreach (var subscription in user.NotifySubscriptions)
            {
                notificationService.Enqueue(new NotificationStoreItem()
                {
                    Subscription = subscription,
                    Request = new NotificationRequest()
                    {
                        Title = translationService.TranslateLocale(request.Title, user.Locale, translationArguments),
                        Body = translationService.TranslateLocale(request.Body, user.Locale, translationArguments),
                        Icon = request.Icon,
                        Data = new Dictionary<string, string>
                        {
                            { "Url", request.Url }
                        }
                    }
                });
            }
        }

        return Task.CompletedTask;
    }

    public async Task VerifyAsync(Guid id, UserVerifyRequest request)
    {
        var user = await userBaseRepository.GetByIdAsync(id);

        if (await userBaseRepository.Set().AnyAsync(x => x.Cellphone == request.Receiver && x.Id != user.Id))
            throw new OperationException(IdentityErrors.UserWithSameCellphoneExists);

        if (await otpService.ValidateAsync(new OtpValidationRequest() { Receiver = request.Receiver, Code = request.Code, Type = request.Type }))
        {
            user.Infos ??= new();

            if (request.Infos is not null)
            {
                foreach (var info in request.Infos)
                {
                    user.Infos[info.Key] = info.Value;
                }
            }

            if (request.Type == OtpType.Email)
            {
                user.Email = request.Receiver;
                user.EmailVerificationTime = DateTime.UtcNow;
            }
            else if (request.Type == OtpType.Cellphone)
            {
                user.Cellphone = request.Receiver;
                user.CellphoneVerificationTime = DateTime.UtcNow;
            }

            await userBaseRepository.UpdateAsync(user);
        }
    }

    public async Task<UserPrivilege> SavePrivilegeAsync(PrivilegeSaveRequest request)
    {
        request.Accessibility = request.Accessibility?.Select(x => x.ToLower()).ToList();
        if (request.Id.HasValue)
        {
            return await userPrivilegeRepository.UpdateWithPatchObjectAsync(request.Id.Value, request);
        }

        return await userPrivilegeRepository.InsertWithPatchObjectAsync(request);
    }
}