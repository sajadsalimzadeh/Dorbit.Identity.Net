using System;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class TokensController(TokenRepository tokenRepository) : BaseController
{
    [HttpPost("{id:guid}/Terminate"), Auth("Tokens-Terminate")]
    public async Task<CommandResult> TerminateAsync([FromRoute] Guid id)
    {
        var token = await tokenRepository.GetByIdAsync(id)
                    ?? throw new NullReferenceException();
        token.State = TokenState.Terminated;
        await tokenRepository.UpdateAsync(token);
        return Succeed();
    }
    
    [HttpPost("Own/{id:guid}/Terminate")]
    public async Task<CommandResult> TerminateOwnAsync([FromRoute] Guid id)
    {
        var userId = GetUserId();
        var token = await tokenRepository.Set().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId)
            ?? throw new NullReferenceException();
        token.State = TokenState.Terminated;
        await tokenRepository.UpdateAsync(token);
        return Succeed();
    }
}