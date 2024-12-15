using System.Security.Cryptography;

public class RsaKeyGenerator
{
    public static string CreateRsaSecurityKey()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            return rsa.ToXmlString(true);
        }
    }
}
