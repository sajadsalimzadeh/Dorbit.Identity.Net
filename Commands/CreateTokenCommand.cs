using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Contracts.Commands;
using Dorbit.Framework.Contracts.Jwts;
using Dorbit.Framework.Extensions;
using Dorbit.Framework.Services;
using Dorbit.Identity.Configs;
using Microsoft.Extensions.Options;

namespace Dorbit.Identity.Commands;

[ServiceRegister]
public class CreateTokenCommand(JwtService jwtService, IOptions<ConfigIdentitySecurity> configIdentitySecret) : Command
{
    public override bool IsRoot { get; } = false;
    public override string Message => "Create Token";

    public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
    {
        yield return new CommandParameter("Id");
        yield return new CommandParameter("Name");
        yield return new CommandParameter("Accesses", "Enter Access (separate with comma) (default:admin)");
        yield return new CommandParameter("Lifetime", "Enter Lifetime (10s, 30m, 2h, 7d, 2w, 3M, 1y) (default:1h)");
    }

    public override Task InvokeAsync(ICommandContext context)
    {
        var accesses = (context.GetArgAsString("Accesses") ?? "admin").Split(',');
        var lifetime = context.GetArgAsString("Lifetime") ?? "1h";
        var lifetimeValue = Convert.ToInt32(lifetime.Substring(0, lifetime.Length - 1));

        var expires = DateTime.UtcNow;
        if (lifetime.EndsWith("s")) expires = expires.AddSeconds(lifetimeValue);
        else if (lifetime.EndsWith("m")) expires = expires.AddMinutes(lifetimeValue);
        else if (lifetime.EndsWith("h")) expires = expires.AddHours(lifetimeValue);
        else if (lifetime.EndsWith("d")) expires = expires.AddDays(lifetimeValue);
        else if (lifetime.EndsWith("w")) expires = expires.AddDays(lifetimeValue * 7);
        else if (lifetime.EndsWith("M")) expires = expires.AddMonths(lifetimeValue);
        else if (lifetime.EndsWith("y")) expires = expires.AddYears(lifetimeValue);

        var csrfToken = Guid.NewGuid().ToString();
        var request = new JwtCreateTokenRequest(csrfToken, configIdentitySecret.Value.Secret.GetDecryptedValue(), expires);
        request.Claims = new ClaimsIdentity();
        foreach (var access in accesses)
        {
            request.Claims.AddClaim(new Claim("access", access));
        }
        request.Claims.AddClaim(new Claim("Id", context.GetArgAsString("Id")));
        request.Claims.AddClaim(new Claim("Name", context.GetArgAsString("Name")));
        
        var accessToken = jwtService.CreateToken(request);
        context.Log($"CsrfToken: {csrfToken}\n");
        context.Log($"AccessToken: {accessToken}\n");

        return Task.CompletedTask;
    }
}