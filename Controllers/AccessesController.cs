using System.Threading.Tasks;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Databases.Entities;

namespace Dorbit.Identity.Controllers;

[Auth("Access")]
public class AccessesController : CrudController<Access, AccessDto, AccessAddDto, AccessEditDto>
{
    [Auth("Access-Read")]
    public override Task<PagedListResult<AccessDto>> SelectAsync()
    {
        return base.SelectAsync();
    }
}