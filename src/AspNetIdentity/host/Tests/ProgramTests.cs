using Microsoft.Extensions.Hosting;
using Xunit;
using Moq;
using Serilog;
using System;
using Microsoft.AspNetCore.Hosting;

namespace IdentityServerHost.Tests
{
    public class ProgramTests : IDisposable
    {
        public void Dispose()
        {
            // Clean up logger after tests
            Log.CloseAndFlush();
        }

        [Fact]
        public void CreateHostBuilder_ReturnsValidHostBuilder()
        {
            // Arrange & Act
            var hostBuilder = Program.CreateHostBuilder(new string[] { });

            // Assert
            Assert.NotNull(hostBuilder);
            Assert.IsType<HostBuilder>(hostBuilder);
        }

        [Fact]
        public void Main_WithValidArgs_ReturnsZero()
        {
            // Arrange
            var args = new string[] { };

            // Act
            var result = Program.Main(args);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void Logger_Configuration_IsValid()
        {
            // Arrange & Act
            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .CreateLogger();

            // Assert
            Assert.NotNull(logger);
        }
    }
}
