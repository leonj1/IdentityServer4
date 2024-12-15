// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Text;
using FluentAssertions;
using IdentityServer.UnitTests.Common;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Xunit;

namespace IdentityServer.UnitTests.Validation.Secrets
{
    public class BasicAuthenticationSecretParsing
    {
        private const string Category = "Secrets - Basic Authentication Secret Parsing";

        private IdentityServerOptions _options;
        private BasicAuthenticationSecretParser _parser;

        public BasicAuthenticationSecretParsing()
        {
            _options = new IdentityServerOptions();
            _parser = new BasicAuthenticationSecretParser(_options, new MockClock());
        }

        [Fact]
        [Trait("Category", Category)]
        public async Task Empty_Request_Should_Return_Null()
        {
            var context = new DefaultHttpContext();

            var secret = await _parser.ParseAsync(context);
            secret.Should().BeNull();
        }

        // Add other test methods here...
    }
}
