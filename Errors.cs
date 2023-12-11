namespace Dorbit.Identity;

internal enum Errors
{
    OtpTryRemainFinished,
    UsernameOrPasswordWrong,
    LoginStrategyIsNotEnabled,
    OtpValidateFailed,
    CorrelationIdIsExpired
}