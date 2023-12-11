using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Models.Users;

public class UserEditRequest : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cellphone { get; set; }
    public string Email { get; set; }
    public bool NeedResetPassword { get; set; }
}