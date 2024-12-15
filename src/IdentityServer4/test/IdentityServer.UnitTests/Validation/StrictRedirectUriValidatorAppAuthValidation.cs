using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class StrictRedirectUriValidatorAppAuthValidation
    {
        private const string Category = "Strict Redirect Uri Validator AppAuth Validation Tests";

        private Client clientWithValidLoopbackRedirectUri = new Client
        {
            RequirePkce = true,
            RedirectUris = new List<string>
            {
                "http://127.0.0.1"
            }
        };

        private Client clientWithNoRedirectUris = new Client
        {
            RequirePkce = true
        };
    }
}
