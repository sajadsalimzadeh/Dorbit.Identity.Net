using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Innofactor.EfCoreJsonValueConverter;

namespace Dorbit.Identity.Entities;

[Table(nameof(Role), Schema = "identity")]
public class Role : FullEntity
{
    [MaxLength(64), Required]
    public string Name { get; set; }
    
    [JsonField]
    public List<string> Accesses { get; set; }
}