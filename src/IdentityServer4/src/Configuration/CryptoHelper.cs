using System;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        var key = CreateRsaSecurityKey();
        Console.WriteLine(key);
    }

    public static string CreateRsaSecurityKey()
    {
        using (var rsa = new RSACryptoServiceProvider(2048))
        {
            return rsa.ToXmlString(true);
        }
    }
}
