using System.Threading.Tasks;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Databases.Entities;
using Microsoft.AspNetCore.OData.Query;

namespace Dorbit.Identity.Controllers;

[Auth("Access")]
public class AccessesController : CrudController<Access, AccessDto, AccessAddDto, AccessEditDto>
{
    [Auth("Access-Read")]
    public override Task<PagedListResult<AccessDto>> Select(ODataQueryOptions<Access> queryOptions)
    {
        return base.Select(queryOptions);
    }
}