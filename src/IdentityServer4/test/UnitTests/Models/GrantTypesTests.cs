using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class GrantTypesTests
    {
        [Fact]
        public void Implicit_Should_Have_Correct_Values()
        {
            GrantTypes.Implicit.Should().BeEquivalentTo(new[] { GrantType.Implicit });
        }

        [Fact]
        public void ImplicitAndClientCredentials_Should_Have_Correct_Values()
        {
            GrantTypes.ImplicitAndClientCredentials.Should()
                .BeEquivalentTo(new[] { GrantType.Implicit, GrantType.ClientCredentials });
        }

        [Fact]
        public void Code_Should_Have_Correct_Values()
        {
            GrantTypes.Code.Should().BeEquivalentTo(new[] { GrantType.AuthorizationCode });
        }

        [Fact]
        public void CodeAndClientCredentials_Should_Have_Correct_Values()
        {
            GrantTypes.CodeAndClientCredentials.Should()
                .BeEquivalentTo(new[] { GrantType.AuthorizationCode, GrantType.ClientCredentials });
        }

        [Fact]
        public void Hybrid_Should_Have_Correct_Values()
        {
            GrantTypes.Hybrid.Should().BeEquivalentTo(new[] { GrantType.Hybrid });
        }

        [Fact]
        public void HybridAndClientCredentials_Should_Have_Correct_Values()
        {
            GrantTypes.HybridAndClientCredentials.Should()
                .BeEquivalentTo(new[] { GrantType.Hybrid, GrantType.ClientCredentials });
        }

        [Fact]
        public void ClientCredentials_Should_Have_Correct_Values()
        {
            GrantTypes.ClientCredentials.Should().BeEquivalentTo(new[] { GrantType.ClientCredentials });
        }

        [Fact]
        public void ResourceOwnerPassword_Should_Have_Correct_Values()
        {
            GrantTypes.ResourceOwnerPassword.Should().BeEquivalentTo(new[] { GrantType.ResourceOwnerPassword });
        }

        [Fact]
        public void ResourceOwnerPasswordAndClientCredentials_Should_Have_Correct_Values()
        {
            GrantTypes.ResourceOwnerPasswordAndClientCredentials.Should()
                .BeEquivalentTo(new[] { GrantType.ResourceOwnerPassword, GrantType.ClientCredentials });
        }

        [Fact]
        public void DeviceFlow_Should_Have_Correct_Values()
        {
            GrantTypes.DeviceFlow.Should().BeEquivalentTo(new[] { GrantType.DeviceFlow });
        }
    }
}
