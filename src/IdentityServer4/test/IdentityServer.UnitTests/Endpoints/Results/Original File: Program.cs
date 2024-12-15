using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of AuthorizeResponse
        bool result = AuthorizeResponse("user123", "password456");
        Console.WriteLine(result);
    }

    static bool AuthorizeResponse(string username, string password)
    {
        // Simulate authorization logic
        return username == "admin" && password == "admin";
    }
}
