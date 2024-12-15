using System;
using MockNamespace; // Assuming the new file is in a different namespace

class Program
{
    static void Main(string[] args)
    {
        // Example usage of MockHttpContextAccessor
        var mockHttpContextAccessor = new MockHttpContextAccessor();
        var context = mockHttpContextAccessor.HttpContext;
        Console.WriteLine(context);
    }
}
