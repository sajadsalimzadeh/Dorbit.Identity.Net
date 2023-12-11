using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Services.Abstractions;

public interface IEntityAccessHandler
{
    string EntityName { get; }
    List<IEntity> GetAllByUserAccess(Guid id);
}