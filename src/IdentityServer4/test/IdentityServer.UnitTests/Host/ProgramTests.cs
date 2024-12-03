using System;
using System.Threading.Tasks;
using IdentityServerHost;
using Microsoft.Extensions.Hosting;
using Xunit;
using Serilog;

namespace IdentityServer.UnitTests.Host
{
    public class ProgramTests : IDisposable
    {
        public ProgramTests()
        {
            // Setup - clear existing logger if any
            Log.CloseAndFlush();
        }

        public void Dispose()
        {
            // Cleanup
            Log.CloseAndFlush();
        }

        [Fact]
        public void Main_WithValidArgs_ReturnsZero()
        {
            // Arrange
            string[] args = new string[] { };

            // Act
            int result = Program.Main(args);

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public void CreateHostBuilder_ReturnsValidHostBuilder()
        {
            // Arrange
            string[] args = new string[] { };

            // Act
            IHostBuilder hostBuilder = Program.CreateHostBuilder(args);

            // Assert
            Assert.NotNull(hostBuilder);
        }

        [Fact]
        public void Main_SetsConsoleTitle()
        {
            // Arrange
            string[] args = new string[] { };

            // Act
            Program.Main(args);

            // Assert
            Assert.Equal("IdentityServer4", Console.Title);
        }
    }
}
