using System;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of GrantTypeValidatingHashSet
        var grantTypes = new GrantTypeValidatingHashSet();
        grantTypes.Add("authorization_code");
        grantTypes.Add("refresh_token");

        if (grantTypes.Contains("authorization_code"))
        {
            Console.WriteLine("Authorization code is valid.");
        }
    }

    public static HashSet<string> GrantTypeValidatingHashSet()
    {
        return new HashSet<string>
        {
            "authorization_code",
            "client_credentials",
            "password"
        };
    }
}
