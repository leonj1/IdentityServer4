using System;
using System.IdentityModel.Tokens.Jwt;

class Program
{
    static void Main(string[] args)
    {
        string token = "your_token_here";
        bool isValid = PrivateKeyJwtSecretValidation(token);
        Console.WriteLine(isValid);
    }

    private static bool PrivateKeyJwtSecretValidation(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new System.Security.Cryptography.RSACryptoServiceProvider().FromXmlString("your_private_key_here"),
                ValidateIssuer = false,
                ValidateAudience = false
            }, out SecurityToken validatedToken);
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }
}
