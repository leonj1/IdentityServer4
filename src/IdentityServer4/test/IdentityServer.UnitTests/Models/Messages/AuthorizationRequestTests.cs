// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Collections.Specialized;
using FluentAssertions;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer.UnitTests.Models.Messages
{
    public class AuthorizationRequestTests
    {
        [Fact]
        public void DefaultConstructor_ShouldInitializeCorrectly()
        {
            // Act
            var request = new AuthorizationRequest();

            // Assert
            request.Parameters.Should().NotBeNull();
            request.RequestObjectValues.Should().NotBeNull();
            request.PromptModes.Should().BeEmpty();
        }

        [Fact]
        public void ConstructorWithValidatedRequest_ShouldMapPropertiesCorrectly()
        {
            // Arrange
            var validatedRequest = new ValidatedAuthorizeRequest
            {
                Client = new Client { ClientId = "test_client" },
                RedirectUri = "https://test.com/callback",
                DisplayMode = "page",
                UiLocales = "en-US",
                LoginHint = "test@test.com",
                Raw = new NameValueCollection { { "test_key", "test_value" } }
            };

            // Act
            var request = new AuthorizationRequest(validatedRequest);

            // Assert
            request.Client.ClientId.Should().Be("test_client");
            request.RedirectUri.Should().Be("https://test.com/callback");
            request.DisplayMode.Should().Be("page");
            request.UiLocales.Should().Be("en-US");
            request.LoginHint.Should().Be("test@test.com");
            request.Parameters["test_key"].Should().Be("test_value");
        }
    }
}
