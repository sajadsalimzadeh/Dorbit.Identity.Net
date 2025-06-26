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
        return Hash.Sha1(password + salt);
    }
    
    public Task<Otp> CreateAsync(OtpCreateRequest request, out string code)
    {
        code = new Random().NextNumber(request.Length);
        var id = Guid.NewGuid();
        return otpRepository.InsertAsync(new Otp()
        {
            Id = id,
            TryRemain = request.TryRemain,
            CodeHash = HashOtp(code, id.ToString()),
            ExpireAt = DateTime.UtcNow.Add(request.Duration),
            IsUsed = false
        });
    }

    public async Task<bool> ValidateAsync(OtpValidateRequest request)
    {
        var otp = await otpRepository.Set().FirstOrDefaultAsync(x => x.Receiver == request.Receiver) ?? throw new NullReferenceException();
        otp.TryRemain--;
        try
        {
            if (otp.TryRemain <= 0) throw new OperationException(IdentityErrors.OtpTryRemainFinished);
            if (otp.IsUsed) throw new OperationException(IdentityErrors.OtpIsUsed);
            if (otp.CodeHash == HashOtp(request.Code, otp.Id.ToString()))
            {
                otp.IsUsed = true;
                return false;
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
        await CreateAsync(new OtpCreateRequest()
        {
            Duration = otpLifetime,
            Length = 5
        }, out var code);
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
                Args = [code]
            });
        }
        else if (request.Type == OtpType.Email)
        {
            await messageManager.SendAsync(new MessageEmailRequest()
            {
                To = request.Receiver,
                Subject = "Login one time password code",
                Body = "Code: {0}",
                Args = [code]
            });
        }
    }
}