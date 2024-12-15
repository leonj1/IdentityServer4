using System;

class Program
{
    static void Main(string[] args)
    {
        string input = "Hello, World!";
        bool isValid = InputLengthRestrictions(input);
        Console.WriteLine(isValid);
    }

    static bool InputLengthRestrictions(string input)
    {
        return input.Length >= 5 && input.Length <= 10;
    }
}
