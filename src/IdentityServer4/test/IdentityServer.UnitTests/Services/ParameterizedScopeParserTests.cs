using FluentAssertions;
using IdentityServer4.Validation;
using IdentityServerHost.Extensions;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using Xunit;

namespace IdentityServer.UnitTests.Services
{
    public class ParameterizedScopeParserTests
    {
        private readonly ParameterizedScopeParser _parser;

        public ParameterizedScopeParserTests()
        {
            _parser = new ParameterizedScopeParser(new NullLogger<DefaultScopeParser>());
        }

        [Fact]
        public void ParseScopeValue_WhenTransactionWithParameter_ShouldParseCorrectly()
        {
            // Arrange
            var scopeValue = "transaction:123";

            // Act
            var result = _parser.ParseScopeValues(new[] { scopeValue });

            // Assert
            result.Succeeded.Should().BeTrue();
            result.ParsedScopes.Count.Should().Be(1);
            var parsedScope = result.ParsedScopes.First();
            parsedScope.ParsedName.Should().Be("transaction");
            parsedScope.ParsedParameter.Should().Be("123");
        }

        [Fact]
        public void ParseScopeValue_WhenTransactionWithoutParameter_ShouldBeIgnored()
        {
            // Arrange
            var scopeValue = "transaction";

            // Act
            var result = _parser.ParseScopeValues(new[] { scopeValue });

            // Assert
            result.Succeeded.Should().BeTrue();
            result.ParsedScopes.Should().BeEmpty();
        }

        [Fact]
        public void ParseScopeValue_WhenInvalidTransactionFormat_ShouldReturnError()
        {
            // Arrange
            var scopeValue = "transaction:";

            // Act
            var result = _parser.ParseScopeValues(new[] { scopeValue });

            // Assert
            result.Succeeded.Should().BeFalse();
            result.Errors.Count.Should().Be(1);
            result.Errors.First().Error.Should().Be("transaction scope missing transaction parameter value");
        }

        [Fact]
        public void ParseScopeValue_WhenNonTransactionScope_ShouldParseAsNormalScope()
        {
            // Arrange
            var scopeValue = "api1";

            // Act
            var result = _parser.ParseScopeValues(new[] { scopeValue });

            // Assert
            result.Succeeded.Should().BeTrue();
            result.ParsedScopes.Count.Should().Be(1);
            var parsedScope = result.ParsedScopes.First();
            parsedScope.ParsedName.Should().Be("api1");
            parsedScope.ParsedParameter.Should().BeNull();
        }

        [Fact]
        public void ParseScopeValue_WhenMixedScopes_ShouldParseCorrectly()
        {
            // Arrange
            var scopeValues = new[] { "api1", "transaction:123", "transaction", "api2" };

            // Act
            var result = _parser.ParseScopeValues(scopeValues);

            // Assert
            result.Succeeded.Should().BeTrue();
            result.ParsedScopes.Count.Should().Be(3);
            
            var scopes = result.ParsedScopes.ToList();
            scopes[0].ParsedName.Should().Be("api1");
            scopes[0].ParsedParameter.Should().BeNull();
            
            scopes[1].ParsedName.Should().Be("transaction");
            scopes[1].ParsedParameter.Should().Be("123");
            
            scopes[2].ParsedName.Should().Be("api2");
            scopes[2].ParsedParameter.Should().BeNull();
        }
    }
}
