using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of the method
        bool isValid = Issuer_uri_should_be_lowercase("https://example.com");
        Console.WriteLine(isValid);
    }

    static bool Issuer_uri_should_be_lowercase(string uri)
    {
        return uri.ToLower() == uri;
    }
}
