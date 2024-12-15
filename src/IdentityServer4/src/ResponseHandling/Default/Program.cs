using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of AuthorizeResponseGenerator
        var response = AuthorizeResponseGenerator.GenerateResponse(true);
        Console.WriteLine(response);
    }
}
