using Dorbit.Framework.Utils.Cryptography;

namespace Dorbit.Identity.Utilities;

public static class HashUtility
{
    public static string HashPassword(string password, string salt)
    {
        return Hash.Sha1(password, salt);
    }
    
    public static string HashOtp(string password, string salt)
    {
        return Hash.Sha1(password, salt);
    }
}