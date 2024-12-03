using System;
using System.Collections.Generic;
using System.Security.Claims;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class TokenValidationResultTests
    {
        [Fact]
        public void TokenValidationResult_DefaultProperties_ShouldBeInitialized()
        {
            // Arrange
            var result = new TokenValidationResult();

            // Assert
            result.Claims.Should().BeNull();
            result.Jwt.Should().BeNull();
            result.ReferenceToken.Should().BeNull();
            result.ReferenceTokenId.Should().BeNull();
            result.RefreshToken.Should().BeNull();
            result.Client.Should().BeNull();
        }

        [Fact]
        public void TokenValidationResult_WithProperties_ShouldHoldValues()
        {
            // Arrange
            var claims = new List<Claim> { new Claim("sub", "123") };
            var jwt = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...";
            var referenceToken = new Token();
            var referenceTokenId = "ref123";
            var refreshToken = new RefreshToken();
            var client = new Client();

            // Act
            var result = new TokenValidationResult
            {
                Claims = claims,
                Jwt = jwt,
                ReferenceToken = referenceToken,
                ReferenceTokenId = referenceTokenId,
                RefreshToken = refreshToken,
                Client = client
            };

            // Assert
            result.Claims.Should().BeSameAs(claims);
            result.Jwt.Should().Be(jwt);
            result.ReferenceToken.Should().BeSameAs(referenceToken);
            result.ReferenceTokenId.Should().Be(referenceTokenId);
            result.RefreshToken.Should().BeSameAs(refreshToken);
            result.Client.Should().BeSameAs(client);
        }

        [Fact]
        public void TokenValidationResult_InheritsFromValidationResult()
        {
            // Arrange & Act
            var result = new TokenValidationResult();

            // Assert
            result.Should().BeAssignableTo<ValidationResult>();
        }
    }
}
