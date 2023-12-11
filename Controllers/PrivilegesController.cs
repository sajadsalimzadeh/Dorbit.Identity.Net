using Dorbit.Framework.Controllers;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dorbit.Identity.Controllers;

public class PrivilegesController: BaseController
{
    private readonly PrivilegeRepository _privilegeRepository;

    public PrivilegesController(PrivilegeRepository privilegeRepository)
    {
        _privilegeRepository = privilegeRepository;
    }

    [HttpGet]
    public ActionResult<IQueryable<Privilege>> Get()
    {
        return Ok(_privilegeRepository.Set());
    }
    
}