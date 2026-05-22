using System;

namespace Dorbit.Identity.Contracts.Users;

public class UserMinimalDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
}