using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage
        try
        {
            NullParameterChecker.CheckNull("example", nameof(example));
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}
