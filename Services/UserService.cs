using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Notifications;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Contracts.Privileges;
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
    TranslationService translationService,
    IOptions<ConfigIdentitySecurity> configIdentitySecurity,
    UserPrivilegeRepository userPrivilegeRepository)
{
    public async Task<User> AddAsync(UserAddRequest request)
    {
        var existsUser = await userRepository.GetByUsernameAsync(request.Username);
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

    public async Task PushNotificationAsync(List<Guid> userIds, NotificationRequest request, Dictionary<string, string> translationArguments = null)
    {
        var users = await userRepository.Set().Where(x => userIds.Contains(x.Id)).Select(x => new User()
        {
            NotifySubscriptions = x.NotifySubscriptions
        }).ToListAsync();
        await PushNotificationAsync(users, request, translationArguments);
    }

    public async Task PushNotificationAsync(List<User> users, NotificationRequest request, Dictionary<string, string> translationArguments = null)
    {
        foreach (var user in users.Where(x => x.NotifySubscriptions.Any(x => x.Type == UserNotifySubscriptionType.WebPush)))
        {
            var webPushClient = new WebPushClient();

            webPushClient.SetVapidDetails(
                subject: "mailto:salimzadehsajad@gmail.com",
                publicKey: configIdentitySecurity.Value.WebPush.PublicKey,
                privateKey: configIdentitySecurity.Value.WebPush.PrivateKey
            );

            foreach (var subscription in user.NotifySubscriptions.Where(x => x.Type == UserNotifySubscriptionType.WebPush))
            {
                try
                {
                    var payload = JsonSerializer.Serialize(new
                    {
                        Notification = new
                        {
                            Title = translationService.TranslateLocale(request.Title, user.Locale, translationArguments),
                            Body = translationService.TranslateLocale(request.Body, user.Locale, translationArguments),
                            request.Icon,
                            Data = new
                            {
                                url = request.Url
                            }
                        }
                    }, JsonSerializerOptions.Web);

                    await webPushClient.SendNotificationAsync(new PushSubscription(subscription.Token, subscription.P256DH, subscription.Auth), payload);
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }

        foreach (var user in users.Where(x => x.NotifySubscriptions.Any(x => x.Type == UserNotifySubscriptionType.Expo)))
        {
            foreach (var subscription in user.NotifySubscriptions.Where(x => x.Type == UserNotifySubscriptionType.Expo))
            {
                try
                {
                    var httpClient = new HttpClient() { BaseAddress = new Uri("https://exp.host/--/api/v2/push/send") };
                    var responseMessage = await httpClient.PostAsJsonAsync("", new NotificationExpoDto()
                    {
                        Token = subscription.Token,
                        Title = translationService.TranslateLocale(request.Title, user.Locale, translationArguments),
                        Body = translationService.TranslateLocale(request.Body, user.Locale, translationArguments),
                        Data = new Dictionary<string, string>
                        {
                            { "url", request.Url }
                        }
                    });
                }
                catch (Exception ex)
                {
                    // ignored
                }
            }
        }
    }

    public async Task VerifyAsync(Guid id, UserVerifyRequest request)
    {
        var user = await userRepository.GetByIdAsync(id);

        if (await userRepository.Set().AnyAsync(x => x.Cellphone == request.Receiver && x.Id != user.Id))
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

            await userRepository.UpdateAsync(user);
        }
    }

    public async Task<UserPrivilege> SavePrivilegeAsync(PrivilegeSaveRequest request)
    {
        request.Accessibility = request.Accessibility?.Select(x => x.ToLower()).ToList();
        var privilege = await userPrivilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == request.UserId);
        if (privilege is null)
        {
            privilege = await userPrivilegeRepository.InsertAsync(request.MapTo<UserPrivilege>());
        }
        else
        {
            privilege = await userPrivilegeRepository.UpdateAsync(request.MapTo(privilege));
        }

        return privilege;
    }
}