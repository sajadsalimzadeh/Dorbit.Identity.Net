using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebPush;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService(
    OtpService otpService,
    UserRepository userRepository,
    TokenRepository tokenRepository,
    IOptions<ConfigIdentitySecurity> configIdentitySecurity,
    UserPrivilegeRepository userPrivilegeRepository)
{

    public async Task<User> AddAsync(UserAddRequest request)
    {
        var existsUser = await userRepository.Set(false).FirstOrDefaultAsync(x => x.Username.ToLower() == request.Username);
        if (existsUser is not null && !existsUser.IsDeleted) throw new OperationException(IdentityErrors.UserExists);
        var entity = request.MapTo(existsUser ?? new User()
        {
            PasswordSalt = Guid.NewGuid().ToString()
        });
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
        return await userRepository.SaveAsync(entity);
    }

    public async Task RemoveAsync(Guid id)
    {
        var admin = await userRepository.GetAdminAsync();
        if (admin.Id == id) throw new OperationException(IdentityErrors.CanNotRemoveAdminUser);
        var transaction = userRepository.DbContext.BeginTransaction();
        await userPrivilegeRepository.BulkDeleteAsync(x => x.UserId == id);
        await tokenRepository.BulkDeleteAsync(x => x.UserId == id);
        await userRepository.DeleteAsync(id);
        await transaction.CommitAsync();
    }

    public async Task<User> ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await userRepository.Set().FirstOrDefaultAsync(x => x.Id == request.Id);
        user.PasswordHash = HashUtil.PasswordV2(request.Password, user.PasswordSalt);
        await userRepository.UpdateAsync(user);
        return user;
    }

    public async Task<User> InActiveAsync(UserDeActiveRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        var admin = await userRepository.GetAdminAsync();
        if (admin.Id == user.Id) throw new OperationException(IdentityErrors.CanNotDeActiveAdmin);
        user.Status = UserStatus.InActive;
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }

    public async Task<User> ActiveAsync(UserActiveRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        user.Status = UserStatus.Active;
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }

    public async Task<User> SetMessageAsync(UserMessageRequest request)
    {
        var user = await userRepository.GetByIdAsync(request.Id);
        user.Message = request.Message;
        return await userRepository.UpdateAsync(user);
    }

    public async Task PushNotificationAsync(List<Guid> userIds, NotificationDto dto)
    {
        var users = await userRepository.Set().Where(x => userIds.Contains(x.Id)).Select(x => new User()
        {
            WebPushSubscriptions = x.WebPushSubscriptions
        }).ToListAsync();
        await PushNotificationAsync(users, dto);
    }

    public async Task PushNotificationAsync(List<User> users, NotificationDto dto)
    {
        var webPushClient = new WebPushClient();

        webPushClient.SetVapidDetails(
            subject: "mailto:salimzadehsajad@gmail.com",
            publicKey: configIdentitySecurity.Value.WebPush.PublicKey,
            privateKey: configIdentitySecurity.Value.WebPush.PrivateKey
        );

        foreach (var user in users)
        {
            if (user.WebPushSubscriptions is null) continue;
            foreach (var webPushToken in user.WebPushSubscriptions)
            {
                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        notification = new
                        {
                            title = dto.Title,
                            body = dto.Body,
                            icon = dto.Icon,
                            data = new
                            {
                                url = dto.Url
                            }
                        }
                    });

                    await webPushClient.SendNotificationAsync(new PushSubscription(webPushToken.Endpoint, webPushToken.P256DH, webPushToken.Auth), payload);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }

    public async Task VerifyCodeAsync(Guid id, UserVerifyRequest request)
    {
        var user = await userRepository.GetByIdAsync(id);


        if (await otpService.ValidateAsync(new OtpValidationRequest() { Receiver = request.Receiver, Code = request.Code, Type = request.Type }))
        {
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

            await userRepository.UpdateAsync(user);
        }
    }
}