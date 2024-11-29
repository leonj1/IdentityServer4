using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using Xunit;
using Moq;

namespace IdentityServer.UnitTests.Services.InMemory
{
    public class InMemoryCorsPolicyServiceTests
    {
        private readonly Mock<ILogger<InMemoryCorsPolicyService>> _logger;
        private readonly List<Client> _clients;
        private readonly InMemoryCorsPolicyService _subject;

        public InMemoryCorsPolicyServiceTests()
        {
            _logger = new Mock<ILogger<InMemoryCorsPolicyService>>();
            _clients = new List<Client>();
            _subject = new InMemoryCorsPolicyService(_logger.Object, _clients);
        }

        [Fact]
        public async Task When_Origin_Is_Not_Allowed_Expect_False()
        {
            // Arrange
            var testOrigin = "https://notallowed.com";

            // Act
            var result = await _subject.IsOriginAllowedAsync(testOrigin);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task When_Origin_Is_Allowed_Expect_True()
        {
            // Arrange
            var testOrigin = "https://allowed.com";
            _clients.Add(new Client 
            { 
                AllowedCorsOrigins = new List<string> { testOrigin }
            });

            // Act
            var result = await _subject.IsOriginAllowedAsync(testOrigin);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task When_Multiple_Origins_Are_Allowed_Expect_True()
        {
            // Arrange
            var testOrigin = "https://allowed2.com";
            _clients.Add(new Client 
            { 
                AllowedCorsOrigins = new List<string> 
                { 
                    "https://allowed1.com",
                    testOrigin,
                    "https://allowed3.com"
                }
            });

            // Act
            var result = await _subject.IsOriginAllowedAsync(testOrigin);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task When_Origin_Case_Differs_Expect_True()
        {
            // Arrange
            var testOrigin = "https://allowed.com";
            _clients.Add(new Client 
            { 
                AllowedCorsOrigins = new List<string> { testOrigin.ToUpper() }
            });

            // Act
            var result = await _subject.IsOriginAllowedAsync(testOrigin.ToLower());

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task When_Clients_Is_Null_Expect_False()
        {
            // Arrange
            var subject = new InMemoryCorsPolicyService(_logger.Object, null);

            // Act
            var result = await subject.IsOriginAllowedAsync("https://any.com");

            // Assert
            result.Should().BeFalse();
        }
    }
}
