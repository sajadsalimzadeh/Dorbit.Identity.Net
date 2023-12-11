﻿using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Framework.Models;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Privileges;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Auth]
public class UsersController : CrudController<User, UserDto, UserAddRequest, UserEditRequest>
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

    public override Task<QueryResult<UserDto>> Add(UserAddRequest dto)
    {
        return _userService.AddAsync(dto).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> Edit(Guid id, UserEditRequest dto)
    {
        return _userService.EditAsync(dto).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    [HttpGet("{id}/Privileges")]
    public async Task<QueryResult<List<string>>> GetAllAccessByUserIdAsync([FromRoute] Guid id)
    {
        var privilege = await _privilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == id);
        return (privilege?.Accesses ?? new List<string>()).ToQueryResult();
    }

    [HttpPost("{id}/Privileges")]
    public async Task<QueryResult<PrivilegeDto>> SaveUserAccessAsync([FromRoute] Guid id, [FromBody] PrivilegeSaveRequest request)
    {
        request.UserId = id;
        return (await _privilegeService.SaveUserAccessAsync(request)).MapTo<PrivilegeDto>().ToQueryResult();
    }
}