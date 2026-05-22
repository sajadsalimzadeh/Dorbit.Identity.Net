using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class OtpsController(OtpRepository otpRepository, OtpService otpService) : BaseController
{
    [HttpGet, Auth("Otp-View")]
    public Task<QueryResult<List<Otp>>> GetAllAsync()
    {
        return otpRepository.Set().ToListAsync().ToQueryResultAsync();
    }
    
    [HttpPost]
    public async Task<CommandResult> SendAsync([FromBody] AuthSendOtpRequest request)
    {
        await otpService.SendAsync(request);
        return Succeed();
    }
}