using System;
using System.Collections.Generic;

namespace MyNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
            var claims = DefaultClaimsService.GetDefaultClaims();
            foreach (var claim in claims)
            {
                Console.WriteLine(claim);
            }
        }

        public static List<string> GetDefaultClaims()
        {
            return new List<string>
            {
                "Claim1",
                "Claim2",
                "Claim3"
            };
        }
    }
}
