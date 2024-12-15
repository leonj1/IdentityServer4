using System;

public static class EndSessionRequestValidator
{
    public static bool Validate(EndSessionRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.UserId))
        {
            return false;
        }
        // Additional validation logic can be added here
        return true;
    }
}
