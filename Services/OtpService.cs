using System;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Utils.Cryptography;
using Dorbit.Identity.Contracts.Otps;
using Dorbit.Identity.Databases.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class OtpService
{
    private readonly OtpRepository _otpRepository;

    public OtpService(OtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
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

    public async Task<bool> ValidateAsync(OtpValidateRequest request)
    {
        var otp = await _otpRepository.GetByIdAsync(request.Id) ?? throw new ArgumentNullException("Otp");
        otp.TryRemain--;
        try
        {
            if (otp.TryRemain <= 0) throw new OperationException(Errors.OtpTryRemainFinished);
            if (otp.CodeHash == HashUtility.HashOtp(request.Code, otp.Id.ToString()))
            {
                otp.IsUsed = true;
                return true;
            }

            return false;
        }
        finally
        {
            await _otpRepository.UpdateAsync(otp);
        }
    }
}