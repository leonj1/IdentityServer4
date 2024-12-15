using System;
using IdentityServer4.Models;

namespace IdentityServer.IntegrationTests.Clients.Setup
{
    public class Secret
    {
        public string Type { get; set; }
        public string Description { get; set; }
        public DateTime? Expiry { get; set; }
        public string Value { get; set; }

        public static Secret FromValue(string value)
        {
            return new Secret
            {
                Value = value,
                Type = IdentityServerConstants.SecretTypes.SharedSecret
            };
        }

        public static Secret FromSha256(string value)
        {
            return new Secret
            {
                Value = Convert.ToBase64String(SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(value))),
                Type = IdentityServerConstants.SecretTypes.Sha256
            };
        }
    }
}
