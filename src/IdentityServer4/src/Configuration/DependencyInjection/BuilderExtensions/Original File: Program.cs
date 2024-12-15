using System;
using Microsoft.IdentityModel.Tokens;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of AddSigningCredential
        var credential = AddSigningCredential();
        Console.WriteLine(credential);
    }

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
