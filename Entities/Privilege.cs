using System.ComponentModel.DataAnnotations.Schema;
using Dorbit.Framework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Dorbit.Identity.Entities;

[Index(nameof(UserId), IsUnique = true)]
public class Privilege : FullEntity
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public List<string> Accesses { get; set; }
}