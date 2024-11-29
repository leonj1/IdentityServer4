using System;
using FluentAssertions;
using IdentityServer4.Hosting;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace IdentityServer4.UnitTests.Hosting
{
    public class EndpointTests
    {
        [Fact]
        public void Constructor_ShouldSetProperties_WhenValidParametersProvided()
        {
            // Arrange
            var name = "TestEndpoint";
            var path = "/test/path";
            var handlerType = typeof(object);

            // Act
            var endpoint = new Endpoint(name, path, handlerType);

            // Assert
            endpoint.Name.Should().Be(name);
            endpoint.Path.Should().Be(new PathString(path));
            endpoint.Handler.Should().Be(handlerType);
        }

        [Fact]
        public void Properties_ShouldBeSettable()
        {
            // Arrange
            var endpoint = new Endpoint("initial", "/initial", typeof(string));
            
            // Act
            endpoint.Name = "updated";
            endpoint.Path = new PathString("/updated");
            endpoint.Handler = typeof(int);

            // Assert
            endpoint.Name.Should().Be("updated");
            endpoint.Path.Should().Be(new PathString("/updated"));
            endpoint.Handler.Should().Be(typeof(int));
        }

        [Fact]
        public void Constructor_ShouldNotThrow_WhenPathIsEmpty()
        {
            // Arrange & Act
            var action = () => new Endpoint("name", "", typeof(object));

            // Assert
            action.Should().NotThrow();
        }

        [Fact]
        public void Constructor_ShouldNotThrow_WhenNameIsEmpty()
        {
            // Arrange & Act
            var action = () => new Endpoint("", "/path", typeof(object));

            // Assert
            action.Should().NotThrow();
        }
    }
}
