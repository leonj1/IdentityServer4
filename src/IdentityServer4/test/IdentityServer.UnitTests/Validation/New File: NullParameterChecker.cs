using System;

public class NullParameterChecker
{
    public static void CheckNull(string param, string paramName)
    {
        if (param == null)
        {
            throw new ArgumentNullException(paramName);
        }
    }
}
