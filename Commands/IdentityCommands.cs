using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class IdentityCommands : Command
{
    private readonly IServiceProvider _serviceProvider;
    
    public override string Message { get; } = "Identity Commands";

    public IdentityCommands(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public override IEnumerable<ICommand> GetSubCommands(ICommandContext context)
    {
        yield return _serviceProvider.GetService<AddUserCommand>();
        yield return _serviceProvider.GetService<ImportAccessCommand>();
        yield return _serviceProvider.GetService<ResetUserPasswordCommand>();
    }

    public override Task Invoke(ICommandContext context)
    {
        return Task.CompletedTask;
    }
}