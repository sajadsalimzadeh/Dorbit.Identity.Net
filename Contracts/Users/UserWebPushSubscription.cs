using System.ComponentModel.DataAnnotations.Schema;

namespace Dorbit.Identity.Contracts.Users;

[NotMapped]
public class UserWebPushSubscription
{
    public string Endpoint { get; set; }

    public string P256DH { get; set; }

    public string Auth { get; set; }
}