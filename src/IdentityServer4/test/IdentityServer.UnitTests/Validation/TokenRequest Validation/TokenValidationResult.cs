using System.Collections.Generic;

public class TokenValidationResult
{
    public bool IsError { get; set; }
    public string Error { get; set; }
    public List<Claim> Claims { get; set; }

    public TokenValidationResult()
    {
        Claims = new List<Claim>();
    }
}
