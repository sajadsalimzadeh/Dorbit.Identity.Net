using Dorbit.Framework.Attributes;
using Dorbit.Identity.Contracts.Accesses;
using Dorbit.Identity.Databases.Repositories;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class AccessService
{
    private readonly AccessRepository _accessRepository;

    public AccessService(AccessRepository accessRepository)
    {
        _accessRepository = accessRepository;
    }
    
    public AccessDto Save(AccessDto request)
    {
        return request;
    }
}