namespace Dorbit.Identity;

internal enum IdentityErrors
{
    OtpTryRemainFinished,
    UsernameOrPasswordWrong,
    OtpTypeNotSupported,
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
    UserExists,
    OtpIsUsed
}