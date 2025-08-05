using System;
using System.Collections.Generic;

namespace Dorbit.Identity.Contracts.Users;

public class UserSendNotificationRequest
{
    public List<Guid> UserIds { get; set; }
    public NotificationDto Notification { get; set; }
}