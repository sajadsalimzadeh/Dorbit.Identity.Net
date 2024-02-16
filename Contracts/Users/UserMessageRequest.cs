using System;

namespace Dorbit.Identity.Contracts.Users;

public class UserMessageRequest
{
    public Guid Id { get; set; }
    public string Message { get; set; }
}