using System;

namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = TargetModelBuilder.BuildTargetModel();
            Console.WriteLine(model);
        }
    }
}
