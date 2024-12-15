using Clients;
using IdentityModel.Client;
using IdentityModel.OidcClient;
using Microsoft.IdentityModel.Logging;
using Serilog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace WindowsConsoleSystemBrowser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IdentityModelEventSource.ShowPII = true;

            if (args.Any())
            {
                await ProcessCallback(args[0]);
            }
            else
            {
                await Run();
            }
        }

        private static async Task ProcessCallback(string args)
        {
            var response = new AuthorizeResponse(args);
            if (!String.IsNullOrWhiteSpace(response.State))
            {
                Console.WriteLine($"Found state: {response.State}");
                var callbackManager = new CallbackManager(response.State);
                await callbackManager.RunClient(args);
            }
            else
            {
                Console.WriteLine("Error: no state on response");
            }
        }

        const string CustomUriScheme = "sample-windows-client";
    }
}
