﻿using System;

namespace Dorbit.Identity.Contracts.Accesses;

public class AccessAddDto
{
    public string Name { get; set; }
    public Guid? ParentId { get; set; }
}