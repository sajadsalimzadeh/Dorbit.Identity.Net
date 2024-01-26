using System;

namespace Dorbit.Identity.Contracts.Accesses;

public class AccessDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}