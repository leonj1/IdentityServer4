using IdentityServer4.Stores;
using Xunit;

namespace UnitTests.Stores
{
    public class PersistedGrantFilterTests
    {
        [Fact]
        public void Creating_Filter_With_SubjectId_Should_Set_Property()
        {
            var filter = new PersistedGrantFilter
            {
                SubjectId = "123"
            };
            
            Assert.Equal("123", filter.SubjectId);
        }

        [Fact]
        public void Creating_Filter_With_SessionId_Should_Set_Property()
        {
            var filter = new PersistedGrantFilter
            {
                SessionId = "session123"
            };
            
            Assert.Equal("session123", filter.SessionId);
        }

        [Fact]
        public void Creating_Filter_With_ClientId_Should_Set_Property()
        {
            var filter = new PersistedGrantFilter
            {
                ClientId = "client123"
            };
            
            Assert.Equal("client123", filter.ClientId);
        }

        [Fact]
        public void Creating_Filter_With_Type_Should_Set_Property()
        {
            var filter = new PersistedGrantFilter
            {
                Type = "reference"
            };
            
            Assert.Equal("reference", filter.Type);
        }

        [Fact]
        public void Creating_Filter_With_Multiple_Properties_Should_Set_All_Properties()
        {
            var filter = new PersistedGrantFilter
            {
                SubjectId = "123",
                SessionId = "session123",
                ClientId = "client123",
                Type = "reference"
            };
            
            Assert.Equal("123", filter.SubjectId);
            Assert.Equal("session123", filter.SessionId);
            Assert.Equal("client123", filter.ClientId);
            Assert.Equal("reference", filter.Type);
        }
    }
}
