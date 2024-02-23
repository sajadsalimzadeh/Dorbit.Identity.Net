namespace Dorbit.Identity;

internal enum Errors
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
    OtpIsInvalid
}