﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dorbit.Framework.Contracts.Results;
using Dorbit.Framework.Controllers;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Filters;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Contracts.Privileges;
using Dorbit.Identity.Contracts.Tokens;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Controllers;

[Auth("User")]
[Route("Identity/[controller]")]
public class UsersController(
    UserService userService,
    UserRepository userRepository,
    IdentityService identityService,
    UserPrivilegeRepository userPrivilegeRepository,
    TokenRepository tokenRepository,
    PrivilegeService privilegeService)
    : CrudController<User, Guid, UserDto, UserAddRequest, UserEditRequest>
{
    [Auth("User-Read")]
    public override Task<PagedListResult<UserDto>> SelectAsync()
    {
        return base.SelectAsync();
    }

    [Auth("User-Read")]
    [HttpGet("Search")]
    public async Task<QueryResult<List<UserDto>>> SelectAsync([FromQuery] UserSearchRequest request)
    {
        var query = userRepository.Set();
        if (!string.IsNullOrEmpty(request.Search)) query = query.Where(x => x.Name.Contains(request.Search) || x.Username.Contains(request.Search));
        if (request.Code.HasValue) query = query.Where(x => x.Code == request.Code);
        var users = (await query.OrderBy(x => x.CreationTime).ToListAsync()).MapTo<List<UserDto>>();
        return users.ToQueryResult();
    }

    [Auth]
    [HttpGet("Own")]
    public Task<QueryResult<UserDto>> GetOwnAsync()
    {
        return userRepository.GetByIdAsync(GetUserId()).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    [Auth]
    [HttpPatch("Own")]
    public Task<QueryResult<UserDto>> EditOwnAsync([FromBody] UserEditOwnRequest request)
    {
        return userRepository.PatchAsync(GetUserId(), request).MapToAsync<User, UserDto>().ToQueryResultAsync();
    }

    public override async Task<CommandResult> Remove(Guid id)
    {
        await userService.RemoveAsync(id);
        return Succeed();
    }

    [HttpPost("Own/ChangePasswordByPassword"), Auth]
    public async Task<CommandResult> ChangePasswordByPasswordAsync([FromBody] AuthChangePasswordByPasswordRequest request)
    {
        await identityService.ChangePasswordByPasswordAsync(request);
        return Succeed();
    }

    [HttpPost("Own/ChangePasswordByOtp"), Auth]
    public async Task<CommandResult> ChangePasswordByOtpAsync([FromBody] AuthChangePasswordByOtpRequest request)
    {
        await identityService.ChangePasswordByOtpAsync(request);
        return Succeed();
    }

    [HttpPost("{id:guid}/ResetPassword"), Auth("User-ResetPassword")]
    public async Task<CommandResult> ResetPasswordAsync([FromRoute] Guid id, [FromBody] UserResetPasswordRequest request)
    {
        request.Id = id;
        var user = await userService.ResetPasswordAsync(request);
        return Succeed();
    }

    [HttpGet("{id:guid}/Privileges"), Auth("Privilege-Read")]
    public Task<QueryResult<List<UserPrivilege>>> GetAllAccessByUserIdAsync([FromRoute] Guid id)
    {
        return userPrivilegeRepository.Set().Where(x => x.UserId == id).ToListAsync().ToQueryResultAsync();
    }

    [HttpPost("{id:guid}/Privileges"), Auth("Privilege")]
    public async Task<QueryResult<PrivilegeDto>> SaveUserAccessAsync([FromRoute] Guid id, [FromBody] PrivilegeSaveRequest request)
    {
        request.UserId = id;
        return (await privilegeService.SaveAsync(request)).MapTo<PrivilegeDto>().ToQueryResult();
    }

    [HttpPost("{id:guid}/DeActive"), Auth("User-DeActive")]
    public async Task<CommandResult> DeActiveAsync([FromRoute] UserDeActiveRequest request)
    {
        return (await userService.InActiveAsync(request)).MapTo<UserDto>().ToQueryResult();
    }

    [HttpPost("{id:guid}/Active"), Auth("User-Active")]
    public async Task<CommandResult> ActiveAsync([FromRoute] UserActiveRequest request)
    {
        return (await userService.ActiveAsync(request)).MapTo<UserDto>().ToQueryResult();
    }

    [HttpGet("{id:guid}/Tokens"), Auth("User-Tokens")]
    public Task<QueryResult<List<TokenDto>>> GetAllTokensAsync([FromRoute] Guid id)
    {
        return tokenRepository.Set()
            .Where(x => x.UserId == id)
            .OrderByDescending(x => x.CreationTime)
            .ToListAsync()
            .MapToAsync<Token, TokenDto>()
            .ToQueryResultAsync();
    }

    [HttpPost("{id:guid}/Message"), Auth("User-Message")]
    public async Task<QueryResult<UserDto>> SetMessageAsync(UserMessageRequest request)
    {
        return (await userService.SetMessageAsync(request)).MapTo<UserDto>().ToQueryResult();
    }
}