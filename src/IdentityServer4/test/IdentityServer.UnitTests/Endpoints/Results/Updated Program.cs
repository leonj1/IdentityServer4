using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of AuthorizeResponse
        bool result = AuthorizationService.AuthorizeResponse("user123", "password456");
        Console.WriteLine(result);
    }
}
