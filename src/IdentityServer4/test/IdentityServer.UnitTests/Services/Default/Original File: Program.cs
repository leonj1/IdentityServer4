using System;

class Program
{
    static void Main(string[] args)
    {
        // Example usage of MockHttpContextAccessor
        var mockHttpContextAccessor = new MockHttpContextAccessor();
        var context = mockHttpContextAccessor.HttpContext;
        Console.WriteLine(context);
    }

    public class MockHttpContextAccessor
    {
        public HttpContext HttpContext { get; set; }
    }
}
