using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using ICommand = Dorbit.Framework.Commands.Abstractions.ICommand;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class IdentityModuleCommand(IServiceProvider serviceProvider) : Command
{
    public override string Message { get; } = "Identity Module";

    public override IEnumerable<ICommand> GetSubCommands(ICommandContext context)
    {
        yield return serviceProvider.GetService<AddUserCommand>();
        yield return serviceProvider.GetService<ImportAccessCommand>();
        yield return serviceProvider.GetService<ResetUserPasswordCommand>();
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        return Task.CompletedTask;
    }
}