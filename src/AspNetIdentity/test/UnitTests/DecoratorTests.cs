using System;
using Xunit;
using IdentityServer4.AspNetIdentity;

namespace IdentityServer4.AspNetIdentity.UnitTests
{
    public class DecoratorTests
    {
        public interface ITestService
        {
            string GetValue();
        }

        public class TestService : ITestService
        {
            public string GetValue() => "test";
        }

        public class DisposableTestService : ITestService, IDisposable
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
            Assert.Same(service, decorator.Instance);
            Assert.Equal("test", decorator.Instance.GetValue());
        }

        [Fact]
        public void GenericDecorator_Should_Wrap_Service()
        {
            // Arrange
            var service = new TestService();
            
            // Act
            var decorator = new Decorator<ITestService, TestService>(service);
            
            // Assert
            Assert.Same(service, decorator.Instance);
            Assert.Equal("test", decorator.Instance.GetValue());
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
            Assert.True(service.IsDisposed);
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
