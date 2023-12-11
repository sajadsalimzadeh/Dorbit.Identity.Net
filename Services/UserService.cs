using Dorbit.Framework.Attributes;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Models.Abstractions;
using Dorbit.Framework.Services.Abstractions;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService : IUserResolver
{
    private readonly UserRepository _userRepository;
    public IUserDto User { get; set; }

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
}