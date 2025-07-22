using System;
using System.Collections.Generic;

namespace Dorbit.Identity.Contracts.Users;

public class UserSendNotificationRequest
{
    public string Title { get; set; }
    public string Body { get; set; }
    public Dictionary<string, string> Data { get; set; }
}