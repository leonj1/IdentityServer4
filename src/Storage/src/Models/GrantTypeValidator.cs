using System;
using System.Collections.Generic;

public class GrantTypeValidator
{
    private HashSet<string> _validGrantTypes;

    public GrantTypeValidator()
    {
        _validGrantTypes = new HashSet<string>
        {
            "authorization_code",
            "client_credentials",
            "password"
        };
    }

    public bool Contains(string grantType)
    {
        return _validGrantTypes.Contains(grantType);
    }
}
