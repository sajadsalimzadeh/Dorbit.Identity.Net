using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

public class AccessesController(AccessRepository accessRepository) : BaseController
{
    [Auth("Access-Read")]
    public Task<QueryResult<List<Access>>> SelectAsync()
    {
        return accessRepository.Set().ToListAsync().ToQueryResultAsync();
    }
}