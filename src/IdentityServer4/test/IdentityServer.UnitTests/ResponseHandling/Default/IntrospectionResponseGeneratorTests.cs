using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.ResponseHandling;
using IdentityServer4.Validation;
using IdentityServer.UnitTests.Common;
using Microsoft.Extensions.Logging;
using Xunit;

namespace IdentityServer.UnitTests.ResponseHandling.Default
{
    public class IntrospectionResponseGeneratorTests
    {
        private readonly IntrospectionResponseGenerator _subject;
        private readonly TestEventService _events;
        private readonly ILogger<IntrospectionResponseGenerator> _logger;

        public IntrospectionResponseGeneratorTests()
        {
            _events = new TestEventService();
            _logger = new LoggerFactory().CreateLogger<IntrospectionResponseGenerator>();
            _subject = new IntrospectionResponseGenerator(_events, _logger);
        }

        [Fact]
        public async Task Should_Return_Active_False_For_Inactive_Token()
        {
            var validationResult = new IntrospectionRequestValidationResult
            {
                IsActive = false,
                Token = "invalid_token"
            };

            var response = await _subject.ProcessAsync(validationResult);

            response.Should().NotBeNull();
            response["active"].Should().Be(false);
            response.Count.Should().Be(1);
        }

        [Fact]
        public async Task Should_Return_Active_True_And_Claims_For_Active_Token()
        {
            var claims = new List<Claim>
            {
                new Claim("sub", "123"),
                new Claim("scope", "api1"),
                new Claim("scope", "api2")
            };

            var validationResult = new IntrospectionRequestValidationResult
            {
                IsActive = true,
                Token = "valid_token",
                Claims = claims,
                Api = new ApiResource("test_api")
                {
                    Scopes = { "api1", "api2" }
                }
            };

            var response = await _subject.ProcessAsync(validationResult);

            response.Should().NotBeNull();
            response["active"].Should().Be(true);
            response["sub"].Should().Be("123");
            response["scope"].Should().Be("api1 api2");
        }

        [Fact]
        public async Task Should_Return_Active_False_When_Token_Contains_No_Matching_Scope()
        {
            var claims = new List<Claim>
            {
                new Claim("sub", "123"),
                new Claim("scope", "api3")
            };

            var validationResult = new IntrospectionRequestValidationResult
            {
                IsActive = true,
                Token = "valid_token",
                Claims = claims,
                Api = new ApiResource("test_api")
                {
                    Scopes = { "api1", "api2" }
                }
            };

            var response = await _subject.ProcessAsync(validationResult);

            response.Should().NotBeNull();
            response["active"].Should().Be(false);
            response.Count.Should().Be(1);
        }
    }
}
