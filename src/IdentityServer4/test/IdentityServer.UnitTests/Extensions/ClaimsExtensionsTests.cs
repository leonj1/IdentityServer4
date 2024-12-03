using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Json;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Extensions;
using Xunit;

namespace IdentityServer.UnitTests.Extensions
{
    public class ClaimsExtensionsTests
    {
        [Fact]
        public void ToClaimsDictionary_WhenNull_ShouldReturnEmptyDictionary()
        {
            IEnumerable<Claim> claims = null;
            var result = claims.ToClaimsDictionary();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ToClaimsDictionary_WithSimpleClaims_ShouldConvertCorrectly()
        {
            var claims = new List<Claim>
            {
                new Claim("name", "John Doe"),
                new Claim("email", "john@example.com")
            };

            var result = claims.ToClaimsDictionary();

            result.Should().HaveCount(2);
            result["name"].Should().Be("John Doe");
            result["email"].Should().Be("john@example.com");
        }

        [Fact]
        public void ToClaimsDictionary_WithDuplicateClaimTypes_ShouldCreateList()
        {
            var claims = new List<Claim>
            {
                new Claim("role", "admin"),
                new Claim("role", "user")
            };

            var result = claims.ToClaimsDictionary();

            result.Should().HaveCount(1);
            result["role"].Should().BeOfType<List<object>>();
            (result["role"] as List<object>).Should().Contain(new[] { "admin", "user" });
        }

        [Fact]
        public void ToClaimsDictionary_WithIntegerClaim_ShouldConvertToNumber()
        {
            var claims = new List<Claim>
            {
                new Claim("age", "25", ClaimValueTypes.Integer)
            };

            var result = claims.ToClaimsDictionary();

            result["age"].Should().Be(25);
        }

        [Fact]
        public void ToClaimsDictionary_WithInt64Claim_ShouldConvertToLong()
        {
            var claims = new List<Claim>
            {
                new Claim("bigNumber", "9223372036854775807", ClaimValueTypes.Integer64)
            };

            var result = claims.ToClaimsDictionary();

            result["bigNumber"].Should().Be(9223372036854775807L);
        }

        [Fact]
        public void ToClaimsDictionary_WithBooleanClaim_ShouldConvertToBoolean()
        {
            var claims = new List<Claim>
            {
                new Claim("isActive", "true", ClaimValueTypes.Boolean)
            };

            var result = claims.ToClaimsDictionary();

            result["isActive"].Should().Be(true);
        }

        [Fact]
        public void ToClaimsDictionary_WithJsonClaim_ShouldDeserializeJson()
        {
            var jsonValue = "{\"key\":\"value\"}";
            var claims = new List<Claim>
            {
                new Claim("jsonData", jsonValue, IdentityServerConstants.ClaimValueTypes.Json)
            };

            var result = claims.ToClaimsDictionary();

            result["jsonData"].Should().BeOfType<JsonElement>();
            var element = (JsonElement)result["jsonData"];
            element.GetProperty("key").GetString().Should().Be("value");
        }

        [Fact]
        public void ToClaimsDictionary_WithInvalidNumberClaim_ShouldKeepOriginalString()
        {
            var claims = new List<Claim>
            {
                new Claim("number", "not-a-number", ClaimValueTypes.Integer)
            };

            var result = claims.ToClaimsDictionary();

            result["number"].Should().Be("not-a-number");
        }

        [Fact]
        public void ToClaimsDictionary_WithInvalidJsonClaim_ShouldKeepOriginalString()
        {
            var claims = new List<Claim>
            {
                new Claim("jsonData", "invalid-json", IdentityServerConstants.ClaimValueTypes.Json)
            };

            var result = claims.ToClaimsDictionary();

            result["jsonData"].Should().Be("invalid-json");
        }
    }
}
