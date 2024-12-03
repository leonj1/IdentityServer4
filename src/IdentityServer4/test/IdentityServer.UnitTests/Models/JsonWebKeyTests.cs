using IdentityServer4.Models;
using FluentAssertions;
using Xunit;

namespace IdentityServer.UnitTests.Models
{
    public class JsonWebKeyTests
    {
        [Fact]
        public void JsonWebKey_Properties_Should_Set_And_Get_Correctly()
        {
            // Arrange
            var jwk = new JsonWebKey
            {
                kty = "RSA",
                use = "sig",
                kid = "key123",
                x5t = "thumb123",
                e = "AQAB",
                n = "n-value",
                x5c = new[] { "cert1", "cert2" },
                alg = "RS256",
                x = "x-value",
                y = "y-value",
                crv = "P-256"
            };

            // Assert
            jwk.kty.Should().Be("RSA");
            jwk.use.Should().Be("sig");
            jwk.kid.Should().Be("key123");
            jwk.x5t.Should().Be("thumb123");
            jwk.e.Should().Be("AQAB");
            jwk.n.Should().Be("n-value");
            jwk.x5c.Should().BeEquivalentTo(new[] { "cert1", "cert2" });
            jwk.alg.Should().Be("RS256");
            jwk.x.Should().Be("x-value");
            jwk.y.Should().Be("y-value");
            jwk.crv.Should().Be("P-256");
        }

        [Fact]
        public void JsonWebKey_Should_Allow_Null_Values()
        {
            // Arrange
            var jwk = new JsonWebKey();

            // Assert
            jwk.kty.Should().BeNull();
            jwk.use.Should().BeNull();
            jwk.kid.Should().BeNull();
            jwk.x5t.Should().BeNull();
            jwk.e.Should().BeNull();
            jwk.n.Should().BeNull();
            jwk.x5c.Should().BeNull();
            jwk.alg.Should().BeNull();
            jwk.x.Should().BeNull();
            jwk.y.Should().BeNull();
            jwk.crv.Should().BeNull();
        }
    }
}
