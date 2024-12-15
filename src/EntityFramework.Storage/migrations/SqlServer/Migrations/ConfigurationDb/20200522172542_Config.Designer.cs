using System;

namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = BuildTargetModel();
            Console.WriteLine(model);
        }

        static string BuildTargetModel()
        {
            return "Target Model";
        }
    }
}
