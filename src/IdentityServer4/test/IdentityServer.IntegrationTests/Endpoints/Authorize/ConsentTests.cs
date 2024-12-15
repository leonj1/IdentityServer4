using System;
using Microsoft.AspNetCore.Builder;

namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            IdentityServerPipeline(builder);
            // Other code...
        }

        static void IdentityServerPipeline(WebApplicationBuilder builder)
        {
            // Function implementation...
        }
    }
}
