using System;

class Program
{
    static void Main(string[] args)
    {
        bool isValid = validator("example");
        Console.WriteLine(isValid);
    }

    static bool validator(string input)
    {
        return !string.IsNullOrEmpty(input) && input.Length > 5;
    }
}
