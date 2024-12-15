using System;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Endpoints.Introspection.Setup
{
    public class Secret
    {
        public byte[] Value { get; set; }
        public string Description { get; set; }

        public static Secret Sha256(string value)
        {
            var secret = new Secret();
            secret.Value = System.Security.Cryptography.SHA256.Create().ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            return secret;
        }
    }
}
