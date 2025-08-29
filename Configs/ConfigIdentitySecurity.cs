﻿using System.Collections.Generic;
using Dorbit.Framework.Contracts.Cryptograpy;
using Dorbit.Identity.Contracts.Otps;

namespace Dorbit.Identity.Configs;

public class ConfigIdentitySecurity
{
    public string PasswordPattern { get; set; } = ".+";
    public int TimeoutInSecond { get; set; } = 300;
    public short OtpTimeoutInSec { get; set; } = 120;
    public bool IgnoreCsrfTokenValidation { get; set; }
    public int MaxActiveTokenCountPerUser { get; set; } = 10;
    public ProtectedProperty Secret { get; set; }
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public ConfigIdentitySecurityWebPush WebPush { get; set; }
    public bool IsCellphoneVerificationRequired { get; set; }
    public bool IsEmailVerificationRequired { get; set; }
}

public class ConfigIdentitySecurityWebPush
{
    public string PublicKey { get; set; }
    public string PrivateKey { get; set; }
}