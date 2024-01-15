
using System;
using System.Collections.Generic;

namespace Dorbit.Identity.Models.Privileges;

public class PrivilegeSaveRequest
{
    public Guid UserId { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public List<string> Accesses { get; set; }
}