using System.IdentityModel.Tokens.Jwt;

public class JwtValidationService
{
    public static bool PrivateKeyJwtSecretValidation(string token)
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
