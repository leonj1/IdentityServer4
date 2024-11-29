using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class ClientTests
    {
        [Fact]
        public void Default_Constructor_Should_Set_Correct_Defaults()
        {
            // Act
            var client = new Client();

            // Assert
            client.Enabled.Should().BeTrue();
            client.ProtocolType.Should().Be(IdentityServerConstants.ProtocolTypes.OpenIdConnect);
            client.RequireClientSecret.Should().BeTrue();
            client.RequireConsent.Should().BeFalse();
            client.AllowRememberConsent.Should().BeTrue();
            client.RequirePkce.Should().BeTrue();
            client.AllowPlainTextPkce.Should().BeFalse();
            client.RequireRequestObject.Should().BeFalse();
            client.AllowAccessTokensViaBrowser.Should().BeFalse();
            client.FrontChannelLogoutSessionRequired.Should().BeTrue();
            client.BackChannelLogoutSessionRequired.Should().BeTrue();
            client.AllowOfflineAccess.Should().BeFalse();
            client.IdentityTokenLifetime.Should().Be(300);
            client.AccessTokenLifetime.Should().Be(3600);
            client.AuthorizationCodeLifetime.Should().Be(300);
            client.AbsoluteRefreshTokenLifetime.Should().Be(2592000);
            client.SlidingRefreshTokenLifetime.Should().Be(1296000);
            client.RefreshTokenUsage.Should().Be(TokenUsage.OneTimeOnly);
            client.EnableLocalLogin.Should().BeTrue();
            client.IncludeJwtId.Should().BeTrue();
            client.AlwaysSendClientClaims.Should().BeFalse();
            client.ClientClaimsPrefix.Should().Be("client_");
            client.DeviceCodeLifetime.Should().Be(300);
        }

        [Fact]
        public void ValidateGrantTypes_Should_Throw_When_GrantTypes_Is_Null()
        {
            // Act & Assert
            Action act = () => Client.ValidateGrantTypes(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ValidateGrantTypes_Should_Throw_When_GrantType_Contains_Space()
        {
            // Arrange
            var grantTypes = new[] { "grant type" };

            // Act & Assert
            Action act = () => Client.ValidateGrantTypes(grantTypes);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Grant types cannot contain spaces");
        }

        [Fact]
        public void ValidateGrantTypes_Should_Throw_When_Duplicate_GrantTypes()
        {
            // Arrange
            var grantTypes = new[] { "grant1", "grant1" };

            // Act & Assert
            Action act = () => Client.ValidateGrantTypes(grantTypes);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Grant types list contains duplicate values");
        }

        [Fact]
        public void ValidateGrantTypes_Should_Not_Throw_With_Valid_Single_GrantType()
        {
            // Arrange
            var grantTypes = new[] { "valid_grant" };

            // Act & Assert
            Action act = () => Client.ValidateGrantTypes(grantTypes);
            act.Should().NotThrow();
        }

        [Theory]
        [InlineData(GrantType.Implicit, GrantType.AuthorizationCode)]
        [InlineData(GrantType.Implicit, GrantType.Hybrid)]
        [InlineData(GrantType.AuthorizationCode, GrantType.Hybrid)]
        public void ValidateGrantTypes_Should_Throw_When_Combining_Forbidden_GrantTypes(string grant1, string grant2)
        {
            // Arrange
            var grantTypes = new[] { grant1, grant2 };

            // Act & Assert
            Action act = () => Client.ValidateGrantTypes(grantTypes);
            act.Should().Throw<InvalidOperationException>()
                .WithMessage($"Grant types list cannot contain both {grant1} and {grant2}");
        }
    }
}
