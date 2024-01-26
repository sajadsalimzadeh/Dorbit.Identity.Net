using System;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Contracts.Accesses;

public class AccessEditDto : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}