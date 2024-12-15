using System;

public class AuthorizationService
{
    public static bool AuthorizeResponse(string username, string password)
    {
        // Simulate authorization logic
        return username == "admin" && password == "admin";
    }
}
