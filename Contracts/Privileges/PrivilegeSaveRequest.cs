using System;
using System.Collections.Generic;

namespace Dorbit.Identity.Contracts.Privileges;

public class PrivilegeSaveRequest
{
    public Guid UserId { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public bool IsAdmin { get; set; }
    public List<string> RoleIds { get; set; }
    public List<string> Accessibility { get; set; }
}