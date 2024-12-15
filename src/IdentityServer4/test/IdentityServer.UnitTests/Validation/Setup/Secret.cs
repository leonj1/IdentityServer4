using System;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer.UnitTests.Validation.Setup
{
    public class Secret
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public static Secret FromSha256(string value)
        {
            return new Secret
            {
                Type = SecretTypes.Sha256,
                Value = value.Sha256()
            };
        }

        public static Secret FromSha512(string value)
        {
            return new Secret
            {
                Type = SecretTypes.Sha512,
                Value = value.Sha512()
            };
        }
    }
}
