using System.Threading.Tasks;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Filters;
using Dorbit.Framework.Models;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Accesses;
using Microsoft.AspNetCore.OData.Query;

namespace Dorbit.Identity.Controllers;

[Auth("Access")]
public class AccessesController : CrudController<Access, AccessDto, AccessAddDto, AccessEditDto>
{
    [Auth("Access-Select")]
    public override Task<PagedListResult<AccessDto>> Select(ODataQueryOptions<Access> queryOptions)
    {
        return base.Select(queryOptions);
    }
}