namespace Dorbit.Identity;

internal enum IdentityErrors
{
    OtpTryRemainFinished,
    UsernameOrPasswordWrong,
    LoginStrategyNotSupported,
    OtpValidateFailed,
    CorrelationIdIsExpired,
    OldPasswordIsInvalid,
    NewPasswordMissMach,
    NewPasswordIsWeak,
    CorrelationIdIsInvalid,
    CanNotDeActiveAdmin,
    CanNotRemoveAdminUser,
    OtpIsInvalid,
    UserNotExists,
    UserExists
}