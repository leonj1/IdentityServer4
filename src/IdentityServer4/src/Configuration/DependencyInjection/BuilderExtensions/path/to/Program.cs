using Microsoft.Extensions.DependencyInjection;

namespace YourNamespace
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var services = new ServiceCollection();
            AddRequiredPlatformServices(services);
            // Rest of your code...
        }
    }
}
