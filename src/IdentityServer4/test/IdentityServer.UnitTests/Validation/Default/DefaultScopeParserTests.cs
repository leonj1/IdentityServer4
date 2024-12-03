using System;
using System.Linq;
using System.Collections.Generic;
using FluentAssertions;
using IdentityServer4.Validation;
using Microsoft.Extensions.Logging;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace IdentityServer.UnitTests.Validation.Default
{
    public class DefaultScopeParserTests
    {
        private readonly DefaultScopeParser _parser;

        public DefaultScopeParserTests()
        {
            _parser = new DefaultScopeParser(NullLogger<DefaultScopeParser>.Instance);
        }

        [Fact]
        public void ParseScopeValues_WhenNull_ThrowsArgumentNullException()
        {
            Action act = () => _parser.ParseScopeValues(null);
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseScopeValues_WithValidScopes_ReturnsSuccessfulResult()
        {
            var scopes = new[] { "openid", "profile", "email" };
            
            var result = _parser.ParseScopeValues(scopes);

            result.Succeeded.Should().BeTrue();
            result.ParsedScopes.Count.Should().Be(3);
            result.Errors.Should().BeEmpty();
            
            result.ParsedScopes.Select(x => x.RawValue)
                .Should().BeEquivalentTo(scopes);
        }

        [Fact]
        public void ParseScopeValue_WithEmptyScope_ReturnsSuccessfulResult()
        {
            var ctx = new DefaultScopeParser.ParseScopeContext("");
            _parser.ParseScopeValue(ctx);
            
            ctx.Succeeded.Should().BeTrue();
            ctx.ParsedName.Should().BeNull();
            ctx.ParsedParameter.Should().BeNull();
        }

        [Fact]
        public void ParseScopeContext_SetParsedValues_WithValidInput_SetsValues()
        {
            var ctx = new DefaultScopeParser.ParseScopeContext("test");
            ctx.SetParsedValues("scope", "param");
            
            ctx.Succeeded.Should().BeTrue();
            ctx.ParsedName.Should().Be("scope");
            ctx.ParsedParameter.Should().Be("param");
            ctx.Error.Should().BeNull();
            ctx.Ignore.Should().BeFalse();
        }

        [Theory]
        [InlineData(null, "param")]
        [InlineData("", "param")]
        [InlineData(" ", "param")]
        [InlineData("scope", null)]
        [InlineData("scope", "")]
        [InlineData("scope", " ")]
        public void ParseScopeContext_SetParsedValues_WithInvalidInput_ThrowsArgumentNullException(string name, string parameter)
        {
            var ctx = new DefaultScopeParser.ParseScopeContext("test");
            Action act = () => ctx.SetParsedValues(name, parameter);
            
            act.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParseScopeContext_SetError_SetsErrorAndClearsOtherValues()
        {
            var ctx = new DefaultScopeParser.ParseScopeContext("test");
            ctx.SetError("error message");
            
            ctx.Succeeded.Should().BeFalse();
            ctx.ParsedName.Should().BeNull();
            ctx.ParsedParameter.Should().BeNull();
            ctx.Error.Should().Be("error message");
            ctx.Ignore.Should().BeFalse();
        }

        [Fact]
        public void ParseScopeContext_SetIgnore_ClearsAllValuesAndSetsIgnore()
        {
            var ctx = new DefaultScopeParser.ParseScopeContext("test");
            ctx.SetIgnore();
            
            ctx.Succeeded.Should().BeFalse();
            ctx.ParsedName.Should().BeNull();
            ctx.ParsedParameter.Should().BeNull();
            ctx.Error.Should().BeNull();
            ctx.Ignore.Should().BeTrue();
        }
    }
}
