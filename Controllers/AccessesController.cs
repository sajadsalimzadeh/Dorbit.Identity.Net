using Dorbit.Framework.Controllers;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Accesses;

namespace Dorbit.Identity.Controllers;

public class AccessesController : CrudController<Access, AccessDto, AccessAddDto, AccessEditDto>
{
}