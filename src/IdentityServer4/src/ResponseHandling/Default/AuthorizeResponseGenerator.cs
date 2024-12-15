public class AuthorizeResponseGenerator
{
    public static string GenerateResponse(bool authorized)
    {
        if (authorized)
        {
            return "Access granted";
        }
        else
        {
            return "Access denied";
        }
    }
}
