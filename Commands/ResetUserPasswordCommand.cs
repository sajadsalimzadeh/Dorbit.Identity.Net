using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Models.Commands;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Services;

namespace Dorbit.Identity.Commands
{
    [ServiceRegister]
    public class ResetUserPasswordCommand : Command
    {
        private readonly UserService _userService;

        public override bool IsRoot { get; } = false;
        public override string Message => "Reset User Password";

        public ResetUserPasswordCommand(UserService userService)
        {
            _userService = userService;
        }

        public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
        {
            yield return new CommandParameter("Username");
            yield return new CommandParameter("Password");
        }

        public override Task Invoke(ICommandContext context)
        {
            return _userService.ResetPasswordAsync(new UserResetPasswordRequest()
            {
                Username = context.Arguments["Username"].ToString(),
                Password = context.Arguments["Password"].ToString(),
            });
        }
    }
}
