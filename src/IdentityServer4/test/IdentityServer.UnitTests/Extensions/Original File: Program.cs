using System;

class Program
{
    static void Main(string[] args)
    {
        bool result = CheckOrigin("http://example.com");
        Console.WriteLine(result);
    }

    static bool CheckOrigin(string origin)
    {
        // Implementation of CheckOrigin method
        return origin.StartsWith("http://example.com");
    }
}
