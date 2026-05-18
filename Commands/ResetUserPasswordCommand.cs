using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Identity.Contracts.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class ResetUserPasswordCommand(UserBaseService userBaseService, UserBaseRepository userBaseRepository) : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Reset User Password";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Username");
        yield return new CommandParameter("Password");
    }

    public override async Task InvokeAsync(ICommandContext context)
    {
        var user = await userBaseRepository.GetByUsernameAsync(context.Arguments["Username"].ToString());
        await userBaseService.ResetPasswordAsync(new UserResetPasswordRequest()
        {
            Id = user.Id,
            Password = context.Arguments["Password"].ToString(),
        });
    }
}