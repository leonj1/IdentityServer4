using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of the method
        bool isValid = UriValidation.Issuer_uri_should_be_lowercase("https://example.com");
        Console.WriteLine(isValid);
    }
}
