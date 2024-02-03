namespace Dorbit.Identity;

internal enum Errors
{
    OtpTryRemainFinished,
    UsernameOrPasswordWrong,
    LoginStrategyNotSupported,
    OtpValidateFailed,
    CorrelationIdIsExpired,
    OldPasswordIsWrong,
    NewPasswordMissMach,
    NewPasswordIsWeak,
    CorrelationIdIsInvalid,
    CanNotDeActiveAdmin,
    CanNotRemoveAdminUser
}