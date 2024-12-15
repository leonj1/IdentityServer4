using System;

class Program
{
    static void Main(string[] args)
    {
        string token = "your_token_here";
        bool isValid = JwtValidationService.PrivateKeyJwtSecretValidation(token);
        Console.WriteLine(isValid);
    }
}
