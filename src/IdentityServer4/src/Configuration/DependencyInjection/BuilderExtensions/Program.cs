using System;

class Program
{
    static void Main(string[] args)
    {
        bool isValid = Validator.Validate("example");
        Console.WriteLine(isValid);
    }
}
