// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Security.Cryptography.X509Certificates;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace IdentityServer.UnitTests.Common
{
    public class TestCertTests
    {
        [Fact]
        public void Load_Should_Return_Valid_Certificate()
        {
            // Act
            var cert = TestCert.Load();

            // Assert
            cert.Should().NotBeNull();
            cert.Should().BeOfType<X509Certificate2>();
            cert.HasPrivateKey.Should().BeTrue();
            cert.Verify().Should().BeTrue();
        }

        [Fact]
        public void LoadSigningCredentials_Should_Return_Valid_Credentials()
        {
            // Act
            var credentials = TestCert.LoadSigningCredentials();

            // Assert
            credentials.Should().NotBeNull();
            credentials.Key.Should().BeOfType<X509SecurityKey>();
            credentials.Algorithm.Should().Be("RS256");
        }
    }
}
