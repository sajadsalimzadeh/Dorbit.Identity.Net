using Dorbit.Framework.Attributes;
using Dorbit.Identity.Entities;
using Dorbit.Identity.Models.Accesses;
using Dorbit.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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