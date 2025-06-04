using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Identity.Configs;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class ValidateTokenCommand(JwtService jwtService, IOptions<ConfigIdentitySecurity> configIdentitySecret) : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Validate Token";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Key", "Key");
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        var result = jwtService.TryValidateToken(context.GetArgAsString("Key"), configIdentitySecret.Value.Secret.GetDecryptedValue());
        if (result) context.Success("Token Is Valid");
        else context.Error("Token Is InValid");
        return Task.CompletedTask;
    }
}