// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using FluentAssertions;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Validation
{
    public class ParsedScopeValidationTests
    {
        [Fact]
        public void ParsedScopeValue_Constructor_Should_Validate_Inputs()
        {
            // Arrange & Act & Assert
            Action act1 = () => new ParsedScopeValue(null);
            act1.Should().Throw<ArgumentNullException>();

            Action act2 = () => new ParsedScopeValue("");
            act2.Should().Throw<ArgumentNullException>();

            Action act3 = () => new ParsedScopeValue(" ");
            act3.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParsedScopeValue_Constructor_Should_Set_Properties_Correctly()
        {
            // Arrange & Act
            var scope = new ParsedScopeValue("scope1");

            // Assert
            scope.RawValue.Should().Be("scope1");
            scope.ParsedName.Should().Be("scope1");
            scope.ParsedParameter.Should().BeNull();
        }

        [Fact]
        public void ParsedScopeValue_Constructor_With_Parameters_Should_Set_Properties_Correctly()
        {
            // Arrange & Act
            var scope = new ParsedScopeValue("raw", "name", "param");

            // Assert
            scope.RawValue.Should().Be("raw");
            scope.ParsedName.Should().Be("name");
            scope.ParsedParameter.Should().Be("param");
        }

        [Fact]
        public void ParsedScopeValidationError_Constructor_Should_Validate_Inputs()
        {
            // Arrange & Act & Assert
            Action act1 = () => new ParsedScopeValidationError(null, "error");
            act1.Should().Throw<ArgumentNullException>();

            Action act2 = () => new ParsedScopeValidationError("scope", null);
            act2.Should().Throw<ArgumentNullException>();

            Action act3 = () => new ParsedScopeValidationError("", "error");
            act3.Should().Throw<ArgumentNullException>();

            Action act4 = () => new ParsedScopeValidationError("scope", "");
            act4.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ParsedScopeValidationError_Constructor_Should_Set_Properties_Correctly()
        {
            // Arrange & Act
            var error = new ParsedScopeValidationError("scope1", "invalid scope");

            // Assert
            error.RawValue.Should().Be("scope1");
            error.Error.Should().Be("invalid scope");
        }
    }
}
