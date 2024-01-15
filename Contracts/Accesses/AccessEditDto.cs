using System;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Models.Accesses;

public class AccessEditDto : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
}