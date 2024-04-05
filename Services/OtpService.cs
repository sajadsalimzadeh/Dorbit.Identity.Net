using System;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Contracts.Messages;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.Extensions.Caching.Distributed;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class OtpService
{
    private readonly OtpRepository _otpRepository;
    private readonly MessageManager _messageManager;
    private readonly IDistributedCache _distributedCache;

    public OtpService(
        OtpRepository otpRepository,
        MessageManager messageManager,
        IDistributedCache distributedCache
    )
    {
        _otpRepository = otpRepository;
        _messageManager = messageManager;
        _distributedCache = distributedCache;
    }

    public Task<Otp> CreateAsync(OtpCreateRequest request, out string code)
    {
        code = new Random().NextNumber(request.Length);
        var id = Guid.NewGuid();
        return _otpRepository.InsertAsync(new Otp()
        {
            Id = id,
            TryRemain = request.TryRemain,
            CodeHash = Hash.Sha1(code, id.ToString()),
            ExpireAt = DateTime.UtcNow.Add(request.Duration),
            IsUsed = false
        });
    }

    public async Task<OtpValidateResponse> ValidateAsync(OtpValidateRequest request)
    {
        var value = await _distributedCache.GetStringAsync(request.Id.ToString());
        if (value is null) throw new OperationException(Errors.CorrelationIdIsExpired);
        
        var otp = await _otpRepository.GetByIdAsync(request.Id) ?? throw new ArgumentNullException("Otp");
        otp.TryRemain--;
        try
        {
            if (otp.TryRemain <= 0) throw new OperationException(Errors.OtpTryRemainFinished);
            if (otp.CodeHash == HashUtility.HashOtp(request.Code, otp.Id.ToString()))
            {
                otp.IsUsed = true;
                return new OtpValidateResponse()
                {
                    Success = true,
                    Value = value
                };
            }

            return new OtpValidateResponse()
            {
                Success = false,
                Value = value
            };
        }
        finally
        {
            await _otpRepository.UpdateAsync(otp);
        }
    }

    public async Task<Otp> SendOtp(AuthSendOtpRequest request)
    {
        var otpLifetime = TimeSpan.FromSeconds(AppIdentity.Setting.Security.OtpTimeoutInSec);
        var otp = await CreateAsync(new OtpCreateRequest()
        {
            Duration = otpLifetime,
            Length = 5
        }, out var code);
        if (request.LoginStrategy == LoginStrategy.Cellphone)
        {
            await _messageManager.SendAsync(new MessageSmsRequest()
            {
                To = request.Value,
                TemplateType = MessageTemplateType.Otp,
                Args = [code]
            });
        }
        else if (request.LoginStrategy == LoginStrategy.Email)
        {
            await _messageManager.SendAsync(new MessageEmailRequest()
            {
                To = request.Value,
                Subject = "Login one time password code",
                Body = "Code: {0}",
                Args = [code]
            });
        }

        await _distributedCache.SetStringAsync(otp.Id.ToString(), request.Value,
            new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = otpLifetime
            });

        return otp;
    }
}