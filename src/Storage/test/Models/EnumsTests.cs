using IdentityServer4.Models;
using Xunit;

namespace IdentityServer4.UnitTests.Models
{
    public class EnumsTests
    {
        [Fact]
        public void SubjectTypes_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)SubjectTypes.Global);
            Assert.Equal(1, (int)SubjectTypes.Ppid);
        }

        [Fact]
        public void AccessTokenType_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)AccessTokenType.Jwt);
            Assert.Equal(1, (int)AccessTokenType.Reference);
        }

        [Fact]
        public void TokenUsage_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)TokenUsage.ReUse);
            Assert.Equal(1, (int)TokenUsage.OneTimeOnly);
        }

        [Fact]
        public void TokenExpiration_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)TokenExpiration.Sliding);
            Assert.Equal(1, (int)TokenExpiration.Absolute);
        }

        [Fact]
        public void CspLevel_Should_Have_Expected_Values()
        {
            Assert.Equal(0, (int)CspLevel.One);
            Assert.Equal(1, (int)CspLevel.Two);
        }
    }
}
