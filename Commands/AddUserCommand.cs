using System.Collections.Generic;
using System.Threading.Tasks;
using Dorbit.Framework.Attributes;
using Dorbit.Framework.Commands;
using Dorbit.Framework.Commands.Abstractions;
using Dorbit.Framework.Models.Commands;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Services;

namespace Dorbit.Identity.Commands
{
    [ServiceRegister]
    public class AddUserCommand : Command
    {
        private readonly UserService _userService;

        public override bool IsRoot { get; } = false;
        public override string Message => "Add User";

        public override IEnumerable<CommandParameter> GetParameters(ICommandContext context)
        {
            yield return new CommandParameter("Name");
            yield return new CommandParameter("Username");
            yield return new CommandParameter("Password");
        }

        public AddUserCommand(UserService userService)
        {
            _userService = userService;
        }

        public override Task Invoke(ICommandContext context)
        {
            return _userService.AddAsync(new UserAddRequest()
            {
                Name = context.Arguments["Name"].ToString(),
                Username = context.Arguments["Username"].ToString(),
                Password = context.Arguments["Password"].ToString(),
            });
        }
    }
}
