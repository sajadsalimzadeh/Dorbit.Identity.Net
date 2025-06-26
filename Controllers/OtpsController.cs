using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class OtpsController(OtpService otpService) : BaseController
{
    [HttpPost]
    public async Task<CommandResult> SendAsync([FromBody] AuthSendOtpRequest request)
    {
        await otpService.SendAsync(request);
        return Succeed();
    }
}