using Microsoft.IdentityModel.Tokens;

public static class SecurityExtensions
{
    public static SecurityTokenDescriptor AddSigningCredential()
    {
        return new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes("secretkey")),
                SecurityAlgorithms.HmacSha256Signature)
        };
    }
}
