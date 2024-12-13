using System;
using Dorbit.Framework.Entities.Abstractions;

namespace Dorbit.Identity.Contracts.Users;

public class UserEditOwnRequest : IEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Cellphone { get; set; }
    public string Thumbnail { get; set; }
}