using Dorbit.Framework.Attributes;
using Dorbit.Framework.Exceptions;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly IUserResolver _userResolver;

    public UserService(UserRepository userRepository, IUserResolver userResolver)
    {
        _userRepository = userRepository;
        _userResolver = userResolver;
    }

    public Task<User> AddAsync(UserAddRequest request)
    {
        var entity = request.MapTo(new User()
        {
            Salt = Guid.NewGuid().ToString()
        });
        entity.PasswordHash = HashUtility.HashPassword(request.Password, entity.Salt);
        return _userRepository.InsertAsync(entity);
    }

    public async Task<User> EditAsync(UserEditRequest request)
    {
        var entity = await _userRepository.GetByIdAsync(request.Id);
        return await _userRepository.UpdateAsync(request.MapTo(entity));
    }

    public async Task<User> RemoveAsync(Guid id)
    {
        return await _userRepository.RemoveAsync(id);
    }

    public async Task ResetPasswordAsync(UserResetPasswordRequest request)
    {
        var user = await _userRepository.Set().FirstOrDefaultAsync(x => x.Username == request.Username);
        await _userRepository.UpdateAsync(request.MapTo(user));
    }
    
    public async Task ChangePasswordAsync(UserChangePasswordRequest request)
    {
        var user = await _userRepository.GetByIdAsync(_userResolver.User.Id);

        if (request.NewPassword != request.RenewPassword)
            throw new OperationException(Errors.NewPasswordMissMach);
        
        
        if (request.NewPassword.Length < 8)
            throw new OperationException(Errors.NewPasswordIsWeak);
        
        if (user.PasswordHash != HashUtility.HashPassword(request.OldPassword, user.Salt))
            throw new OperationException(Errors.OldPasswordIsWrong);

        user.Salt = Guid.NewGuid().ToString();
        user.PasswordHash = HashUtility.HashPassword(request.NewPassword, user.Salt);
        
        await _userRepository.UpdateAsync(user);
    }
}