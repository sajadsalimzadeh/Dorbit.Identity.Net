using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Framework.Models;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Privileges;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Auth("User")]
public class UsersController : CrudController<User, UserIdentityDto, UserAddRequest, UserEditRequest>
{
    private readonly UserService _userService;
    private readonly PrivilegeRepository _privilegeRepository;
    private readonly PrivilegeService _privilegeService;

    public UsersController(
        UserService userService,
        PrivilegeRepository privilegeRepository,
        PrivilegeService privilegeService)
    {
        _userService = userService;
        _privilegeRepository = privilegeRepository;
        _privilegeService = privilegeService;
    }

    [Auth("User-Read")]
    public override Task<PagedListResult<UserIdentityDto>> Select(ODataQueryOptions<User> queryOptions)
    {
        return base.Select(queryOptions);
    }

    public override Task<QueryResult<UserIdentityDto>> Add(UserAddRequest dto)
    {
        return _userService.AddAsync(dto).MapAsync<User, UserIdentityDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserIdentityDto>> Edit(Guid id, UserEditRequest dto)
    {
        return _userService.EditAsync(dto).MapAsync<User, UserIdentityDto>().ToQueryResultAsync();
    }

    [HttpPost("ChangePassword"), Auth]
    public async Task<CommandResult> ChangePassword([FromBody] UserChangePasswordRequest request)
    {
        await _userService.ChangePasswordAsync(request);
        return Succeed();
    }

    [HttpGet("{id}/Privileges"), Auth("Privilege-Read")]
    public async Task<QueryResult<List<string>>> GetAllAccessByUserIdAsync([FromRoute] Guid id)
    {
        var privilege = await _privilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == id);
        return (privilege?.Accesses ?? new List<string>()).ToQueryResult();
    }

    [HttpPost("{id}/Privileges"), Auth("Privilege")]
    public async Task<QueryResult<PrivilegeDto>> SaveUserAccessAsync([FromRoute] Guid id, [FromBody] PrivilegeSaveRequest request)
    {
        request.UserId = id;
        return (await _privilegeService.SaveAsync(request)).MapTo<PrivilegeDto>().ToQueryResult();
    }
}