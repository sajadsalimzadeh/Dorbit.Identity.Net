using System.Threading.Tasks;
using Dorbit.Identity.Contracts.Users;

namespace Dorbit.Identity.Services.Abstractions;

public interface IUserServiceWrapper
{
    Task OnAddExecutingAsync(UserAddRequest request);
}