using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Validation.Models
{
    public class GrantValidationResultTests
    {
        [Fact]
        public void Constructor_with_no_subject_should_create_successful_result()
        {
            var result = new GrantValidationResult();
            
            result.IsError.Should().BeFalse();
            result.Subject.Should().BeNull();
            result.CustomResponse.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_with_subject_and_auth_method_should_create_successful_result()
        {
            var result = new GrantValidationResult(
                "123",
                "custom",
                authTime: new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc));

            result.IsError.Should().BeFalse();
            result.Subject.Should().NotBeNull();
            result.Subject.FindFirst(JwtClaimTypes.Subject).Value.Should().Be("123");
            result.Subject.FindFirst(JwtClaimTypes.AuthenticationMethod).Value.Should().Be("custom");
            result.Subject.FindFirst(JwtClaimTypes.AuthenticationTime).Value.Should().Be("1672531200");
        }

        [Fact]
        public void Constructor_with_error_should_create_error_result()
        {
            var result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_grant_error");

            result.IsError.Should().BeTrue();
            result.Error.Should().Be("invalid_grant");
            result.ErrorDescription.Should().Be("invalid_grant_error");
        }

        [Fact]
        public void Constructor_with_claims_should_include_claims_in_subject()
        {
            var claims = new[]
            {
                new Claim("role", "admin"),
                new Claim("permission", "read")
            };

            var result = new GrantValidationResult(
                "123",
                "custom",
                claims: claims);

            result.Subject.HasClaim("role", "admin").Should().BeTrue();
            result.Subject.HasClaim("permission", "read").Should().BeTrue();
        }

        [Fact]
        public void Constructor_with_custom_response_should_include_custom_response()
        {
            var customResponse = new Dictionary<string, object>
            {
                { "custom_key", "custom_value" }
            };

            var result = new GrantValidationResult(
                "123",
                "custom",
                customResponse: customResponse);

            result.CustomResponse.Should().ContainKey("custom_key");
            result.CustomResponse["custom_key"].Should().Be("custom_value");
        }

        [Fact]
        public void Constructor_with_invalid_principal_should_throw()
        {
            var principal = new ClaimsPrincipal(new ClaimsIdentity[]
            {
                new ClaimsIdentity(),
                new ClaimsIdentity()
            });

            Action act = () => new GrantValidationResult(principal);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("only a single identity supported");
        }

        [Fact]
        public void Constructor_with_missing_required_claims_should_throw()
        {
            var identity = new ClaimsIdentity("custom");
            var principal = new ClaimsPrincipal(identity);

            Action act = () => new GrantValidationResult(principal);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("sub claim is missing");
        }
    }
}
