using System;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class GrantTypesValidation_EmptyShouldBeAllowed
    {
        [Fact]
        public void empty_should_be_allowed()
        {
            var client = new Client();
            client.AllowedGrantTypes = new List<string>();
        }
    }
}
