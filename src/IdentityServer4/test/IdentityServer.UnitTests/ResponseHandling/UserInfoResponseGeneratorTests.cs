using System;
using YourNamespace; // Replace with your actual namespace

class Program
{
    static void Main(string[] args)
    {
        // Example usage of MockProfileService
        var mockProfile = MockProfileService.GetProfile();
        Console.WriteLine(mockProfile);
    }
}
