using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Roles;
using Dorbit.Identity.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class RolesController : CrudController<Role>
{
    [HttpGet("Minimal"), Auth("Role-ViewMinimal")]
    public Task<QueryResult<List<RoleMinimalDto>>> GetAllMinimalAsync()
    {
        return Repository.Set().Select(x => new RoleMinimalDto()
        {
            Id = x.Id,
            Name = x.Name,
        }).ToListAsync().ToQueryResultAsync();
    }
}