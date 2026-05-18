using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Services;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class AddUserCommand(UserBaseService userBaseService) : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Add User";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Name");
        yield return new CommandParameter("Username");
        yield return new CommandParameter("Password");
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        return userBaseService.AddAsync(new UserAddRequest()
        {
            Name = context.Arguments["Name"].ToString(),
            Username = context.Arguments["Username"].ToString(),
            Password = context.Arguments["Password"].ToString(),
        });
    }
}