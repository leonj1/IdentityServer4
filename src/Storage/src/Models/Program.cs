using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of GrantTypeValidatingHashSet
        var grantTypes = new GrantTypeValidator();
        if (grantTypes.Contains("authorization_code"))
        {
            Console.WriteLine("Authorization code is valid.");
        }
    }
}
