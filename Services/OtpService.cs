using System;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Messages;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Configs;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class OtpService(
    OtpRepository otpRepository,
    MessageManager messageManager,
    SettingService settingService,
    IOptions<ConfigIdentitySecurity> configSecurityOptions)
{
    private static string HashOtp(string password, string salt)
    {
        return HashUtil.Sha1(password + salt);
    }

    public async Task<Otp> CreateAsync(OtpCreateRequest request)
    {
        var otp = await otpRepository.Set().FirstOrDefaultAsync(x => x.Receiver == request.Receiver);
        var code = "0123456789".Random(request.Length);
        if (otp is null)
        {
            otp = await otpRepository.InsertAsync(new Otp()
            {
                Code = code,
                Type = request.Type,
                Receiver = request.Receiver,
                TryRemain = request.TryRemain,
                ExpireAt = DateTime.UtcNow.Add(request.Duration),
                IsUsed = false
            });
        }
        else
        {
            otp.Code = code;
            otp.TryRemain = 5;
            otp.Type = request.Type;
            otp.ExpireAt = DateTime.UtcNow.Add(request.Duration);
            otp.IsUsed = false;
            await otpRepository.UpdateAsync(otp);
        }

        return otp;
    }

    public async Task<bool> ValidateAsync(OtpValidationRequest request)
    {
        var otp = await otpRepository.Set().FirstOrDefaultAsync(x => x.Receiver == request.Receiver && x.Type == request.Type) ??
                  throw new OperationException(IdentityErrors.EntityNotFound);
        otp.TryRemain--;
        try
        {
            if (otp.TryRemain <= 0) throw new OperationException(IdentityErrors.OtpTryRemainFinished);
            if (otp.IsUsed) throw new OperationException(IdentityErrors.OtpIsUsed);
            if (otp.Code == request.Code)
            {
                otp.IsUsed = true;
                return true;
            }

            return false;
        }
        finally
        {
            await otpRepository.UpdateAsync(otp);
        }
    }

    public async Task SendAsync(AuthSendOtpRequest request)
    {
        var otpLifetime = TimeSpan.FromSeconds(configSecurityOptions.Value.OtpTimeoutInSec);
        var otp = await CreateAsync(new OtpCreateRequest()
        {
            Receiver = request.Receiver,
            Type = request.Type,
            Duration = otpLifetime,
            Length = 5
        });
        if (request.Type == OtpType.Cellphone)
        {
            var settingIdentityOtp = settingService.Get(new SettingIdentityOtp()
            {
                TemplateCode = "otp"
            });
            await messageManager.SendAsync(new MessageSmsRequest()
            {
                To = request.Receiver,
                TemplateType = settingIdentityOtp.TemplateCode,
                Args = [otp.Code]
            });
        }
        else if (request.Type == OtpType.Email)
        {
            await messageManager.SendAsync(new MessageEmailRequest()
            {
                To = request.Receiver,
                Subject = "One time password",
                Body = "Your one time password is: {0}",
                Args = [otp.Code]
            });
        }
    }
}