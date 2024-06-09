using System;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

public class OtpsController(OtpService otpService) : BaseController
{
    [HttpPost]
    public async Task<QueryResult<Guid>> SendAsync([FromBody] AuthSendOtpRequest request)
    {
        var otp = await otpService.SendOtp(request);
        return otp.Id.ToQueryResult();
    }
}