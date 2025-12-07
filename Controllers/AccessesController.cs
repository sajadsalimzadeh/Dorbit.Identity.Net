using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class AccessesController(AccessRepository accessRepository) : BaseController
{
    [HttpGet, Auth]
    public Task<QueryResult<List<Access>>> GetAllAsync()
    {
        return accessRepository.GetAllAccessibilityAsync().ToQueryResultAsync();
    }
    
    [HttpGet("Dictionary"), Auth]
    public Task<QueryResult<Dictionary<string, HashSet<string>>>> GetDictionaryAsync()
    {
        return accessRepository.GetAccessibilityDictionaryAsync().ToQueryResultAsync();
    }
}