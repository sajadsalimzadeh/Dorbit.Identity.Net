using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Databases.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Auth("User")]
public class UsersController : CrudController<User, UserDto, UserAddRequest, UserEditRequest>
{
    private readonly UserService _userService;
    private readonly UserRepository _userRepository;
    private readonly PrivilegeRepository _privilegeRepository;
    private readonly PrivilegeService _privilegeService;

    public UsersController(
        UserService userService,
        UserRepository userRepository,
        PrivilegeRepository privilegeRepository,
        PrivilegeService privilegeService)
    {
        _userService = userService;
        _userRepository = userRepository;
        _privilegeRepository = privilegeRepository;
        _privilegeService = privilegeService;
    }

    [Auth("User-Read")]
    public override async Task<PagedListResult<UserDto>> SelectAsync()
    {
        var result = await base.SelectAsync();
        foreach (var userDto in result.Data)
        {
            var allAccesses = await _privilegeRepository.Set()
                .Where(x => x.UserId == userDto.Id)
                .Select(x => x.Accesses)
                .ToListAsyncWithCache($"Privileges-Accesses-{userDto.Id}", TimeSpan.FromSeconds(5));
            userDto.Accesses = allAccesses.SelectMany(x => x);
        }

        return result;
    }
    
    [Auth]
    [HttpGet("Own")]
    public Task<QueryResult<UserDto>> GetOwnAsync()
    {
        return _userRepository.GetByIdAsync(UserId).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> AddAsync(UserAddRequest dto)
    {
        return _userService.AddAsync(dto).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> EditAsync(Guid id, UserEditRequest dto)
    {
        return _userService.EditAsync(dto).MapAsync<User, UserDto>().ToQueryResultAsync();
    }
    
    [Auth]
    [HttpPatch("Own")]
    public Task<QueryResult<UserDto>> EditOwnAsync(UserEditRequest dto)
    {
        dto.Id = UserId;
        return _userService.EditAsync(dto).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> Remove(Guid id)
    {
        return _userService.RemoveAsync(id).MapAsync<User, UserDto>().ToQueryResultAsync();
    }

    [HttpPost("ChangePassword"), Auth]
    public async Task<CommandResult> ChangePasswordAsync([FromBody] UserChangePasswordRequest request)
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

    [HttpPost("{id}/DeActive"), Auth("User-DeActive")]
    public async Task<QueryResult<UserDto>> DeActiveAsync([FromRoute] Guid id)
    {
        return (await _userService.DeActiveAsync(id)).MapTo<UserDto>().ToQueryResult();
    }

    [HttpPost("{id}/Active"), Auth("User-Active")]
    public async Task<QueryResult<UserDto>> ActiveAsync([FromRoute] Guid id)
    {
        return (await _userService.ActiveAsync(id)).MapTo<UserDto>().ToQueryResult();
    }
}