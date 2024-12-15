using System;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServerHost
{
    public class SeedDataInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var seedData = new SeedData();
            seedData.EnsureSeedData(serviceProvider);
        }
    }
}
