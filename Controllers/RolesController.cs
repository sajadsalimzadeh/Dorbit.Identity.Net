using Dorbit.Framework.Controllers;
using Dorbit.Identity.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

[Route("Identity/[controller]")]
public class RolesController : CrudController<Role>
{
}