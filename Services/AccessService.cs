using Dorbit.Attributes;
using Dorbit.Identity.Models.Accesses;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class AccessService
{
    public AccessDto Save(AccessDto request)
    {
        return request;
    }
}