using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage
        Null_Parameter("example");
    }

    static void Null_Parameter(string param)
    {
        if (param == null)
        {
            throw new ArgumentNullException(nameof(param));
        }
        Console.WriteLine($"Parameter: {param}");
    }
}
