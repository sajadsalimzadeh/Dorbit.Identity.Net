using Dorbit.Attributes;
using Dorbit.Exceptions;
using Dorbit.Identity.Enums;
using Dorbit.Identity.Models.Users;
using Dorbit.Identity.Repositories;
using Dorbit.Identity.Services.Abstractions;
using Dorbit.Models.Messages;
using Dorbit.Services;
using Dorbit.Utils.Cryptography;

namespace Dorbit.Identity.Services;

[ServiceRegister]
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly MessageManager _messageManager;

    public UserService(UserRepository userRepository, MessageManager messageManager)
    {
        _userRepository = userRepository;
        _messageManager = messageManager;
    }

    private string HashPassword(string password, string salt)
    {
        return Hash.SHA1(password, salt);
    }

    public UserLoginRequest Login(UserLoginRequest request)
    {
        var user = _userRepository.Set().FirstOrDefault(x => x.Username == request.Username) ??
                   throw new OperationException(Errors.UsernameOrPasswordWrong);
        if (user.IsTwoFactorAuthenticationEnable)
        {
            if (request.LoginStrategy == UserLoginStrategy.None)
            {
                if (user.CellphoneLoginEnable) request.LoginStrategy = UserLoginStrategy.Cellphone;
                else if (user.EmailLoginEnable) request.LoginStrategy = UserLoginStrategy.Email;
                else if (user.AuthenticatorLoginEnable) request.LoginStrategy = UserLoginStrategy.Authenticator;
            }

            if (request.LoginStrategy == UserLoginStrategy.Cellphone)
            {
                _messageManager.Send(new MessageSmsRequest()
                {
                    To = user.Cellphone,

                });
            }
        }

        var hash = HashPassword(request.Password, user.Salt);
        if(user.PasswordHash != hash) 
            throw new OperationException(Errors.UsernameOrPasswordWrong);
        
        
    }

    public UserLoginCompleteResponse LoginWithOtp(UserLoginWithOtpRequest request)
}