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
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
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
    [HttpGet("Search")]
    public async Task<QueryResult<List<UserDto>>> SelectAsync([FromQuery] UserSearchRequest request)
    {
        var query = _userRepository.Set();
        if (!string.IsNullOrEmpty(request.Search)) query = query.Where(x => x.Name.Contains(request.Search) || x.Username.Contains(request.Search));
        if (request.Code.HasValue) query = query.Where(x => x.Code == request.Code);
        var users = (await query.ToListAsync()).MapTo<List<UserDto>>();
        foreach (var userDto in users)
        {
            var allAccesses = await _privilegeRepository.Set()
                .Where(x => x.UserId == userDto.Id)
                .Select(x => x.Accesses)
                .ToListAsyncWithCache($"Privileges-Accesses-{userDto.Id}", TimeSpan.FromSeconds(5));
            userDto.Accesses = allAccesses.SelectMany(x => x);
        }

        return users.ToQueryResult();
    }

    [Auth]
    [HttpGet("Own")]
    public Task<QueryResult<UserDto>> GetOwnAsync()
    {
        return _userRepository.GetByIdAsync(UserId).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> AddAsync(UserAddRequest request)
    {
        return _userService.AddAsync(request).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> EditAsync(Guid id, UserEditRequest request)
    {
        return _userService.EditAsync(request).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    [Auth]
    [HttpPatch("Own")]
    public Task<QueryResult<UserDto>> EditOwnAsync(UserEditRequest dto)
    {
        dto.Id = UserId;
        return _userService.EditAsync(dto).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override Task<QueryResult<UserDto>> Remove(Guid id)
    {
        return _userService.RemoveAsync(id).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    [HttpPost("Own/ChangePassword"), Auth]
    public async Task<CommandResult> ChangePasswordAsync([FromBody] UserChangePasswordRequest request)
    {
        await _userService.ChangePasswordAsync(request);
        return Succeed();
    }

    [HttpPost("{id:guid}/ResetPassword"), Auth("User-ResetPassword")]
    public async Task<QueryResult<UserDto>> ResetPasswordAsync([FromRoute] Guid id, [FromBody] UserResetPasswordRequest request)
    {
        request.Id = id;
        var user = await _userService.ResetPasswordAsync(request);
        return user.MapTo<UserDto>().ToQueryResult();
    }

    [HttpGet("{id:guid}/Privileges"), Auth("Privilege-Read")]
    public async Task<QueryResult<List<string>>> GetAllAccessByUserIdAsync([FromRoute] Guid id)
    {
        var privilege = await _privilegeRepository.Set().FirstOrDefaultAsync(x => x.UserId == id);
        return (privilege?.Accesses ?? new List<string>()).ToQueryResult();
    }

    [HttpPost("{id:guid}/Privileges"), Auth("Privilege")]
    public async Task<QueryResult<PrivilegeDto>> SaveUserAccessAsync([FromRoute] Guid id, [FromBody] PrivilegeSaveRequest request)
    {
        request.UserId = id;
        return (await _privilegeService.SaveAsync(request)).MapTo<PrivilegeDto>().ToQueryResult();
    }

    [HttpPost("{id:guid}/DeActive"), Auth("User-DeActive")]
    public async Task<QueryResult<UserDto>> DeActiveAsync([FromRoute] UserDeActiveRequest request)
    {
        return (await _userService.DeActiveAsync(request)).MapTo<UserDto>().ToQueryResult();
    }

    [HttpPost("{id:guid}/Active"), Auth("User-Active")]
    public async Task<QueryResult<UserDto>> ActiveAsync([FromRoute] UserActiveRequest request)
    {
        return (await _userService.ActiveAsync(request)).MapTo<UserDto>().ToQueryResult();
    }

    [HttpPost("{id:guid}/Message"), Auth("User-Message")]
    public async Task<QueryResult<UserDto>> SetMessageAsync(UserMessageRequest request)
    {
        return (await _userService.SetMessageAsync(request)).MapTo<UserDto>().ToQueryResult();
    }
}