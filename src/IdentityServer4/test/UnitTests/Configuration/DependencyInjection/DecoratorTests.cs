using System;
using FluentAssertions;
using IdentityServer4.Configuration.DependencyInjection;
using Xunit;

namespace IdentityServer4.UnitTests.Configuration.DependencyInjection
{
    public class DecoratorTests
    {
        private interface ITestService
        {
            string GetValue();
        }

        private class TestService : ITestService
        {
            public string GetValue() => "test";
        }

        private class DisposableTestService : ITestService, IDisposable
        {
            public bool IsDisposed { get; private set; }

            public string GetValue() => "disposable";

            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        [Fact]
        public void Decorator_Should_Wrap_Service()
        {
            // Arrange
            var service = new TestService();

            // Act
            var decorator = new Decorator<ITestService>(service);

            // Assert
            decorator.Instance.Should().Be(service);
            decorator.Instance.GetValue().Should().Be("test");
        }

        [Fact]
        public void TypedDecorator_Should_Wrap_Service()
        {
            // Arrange
            var service = new TestService();

            // Act
            var decorator = new Decorator<ITestService, TestService>(service);

            // Assert
            decorator.Instance.Should().Be(service);
            decorator.Instance.GetValue().Should().Be("test");
        }

        [Fact]
        public void DisposableDecorator_Should_Dispose_Service()
        {
            // Arrange
            var service = new DisposableTestService();
            var decorator = new DisposableDecorator<ITestService>(service);

            // Act
            decorator.Dispose();

            // Assert
            service.IsDisposed.Should().BeTrue();
        }

        [Fact]
        public void DisposableDecorator_Should_Handle_NonDisposable_Service()
        {
            // Arrange
            var service = new TestService();
            var decorator = new DisposableDecorator<ITestService>(service);

            // Act & Assert
            decorator.Dispose(); // Should not throw
        }
    }
}
