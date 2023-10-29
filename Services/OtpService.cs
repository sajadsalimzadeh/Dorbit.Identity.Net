using Dorbit.Attributes;
using Dorbit.Exceptions;
using Dorbit.Extensions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Otps;
using Dorbit.Identity.Repositories;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class OtpService
{
    private readonly OtpRepository _otpRepository;

    public OtpService(OtpRepository otpRepository)
    {
        _otpRepository = otpRepository;
    }

    public Otp Create(OtpCreateRequest request)
    {
        return _otpRepository.Insert(new Otp()
        {
            TryRemain = request.TryRemain,
            Code = new Random().NextNumber(request.Length),
            ExpireAt = DateTime.UtcNow.Add(request.Duration),
            IsUsed = false
        });
    }

    public bool Validate(OtpValidateRequest request)
    {
        var otp = _otpRepository.GetById(request.Id) ?? throw new ArgumentNullException("Otp");
        otp.TryRemain--;
        try
        {
            if (otp.TryRemain <= 0) throw new OperationException(Errors.OtpTryRemainFinished);
            if (otp.Code == request.Code)
            {
                otp.IsUsed = true;
                return true;
            }

            return false;
        }
        finally
        {
            _otpRepository.Update(otp);
        }
    }
}