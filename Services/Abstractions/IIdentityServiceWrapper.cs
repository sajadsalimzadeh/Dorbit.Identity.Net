using System.Threading.Tasks;
using Dorbit.Identity.Contracts.Auth;
using Dorbit.Identity.Entities;

namespace Dorbit.Identity.Services.Abstractions;

public interface IIdentityServiceWrapper
{
    Task OnLoginExecutingAsync(User user);
}