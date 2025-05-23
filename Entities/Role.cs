﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Dorbit.Framework.Utils.Json;

namespace Dorbit.Identity.Entities;

[Table(nameof(Role), Schema = "identity")]
public class Role : FullEntity
{
    [MaxLength(64), Required]
    public string Name { get; set; }
    
    [JsonField]
    public List<string> Accessibility { get; set; }
}