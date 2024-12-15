using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ScopeMapperProfileTests
    {
        [Fact]
        public void ScopeAutomapperConfigurationIsValid()
        {
            ScopeMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ScopeMapperProfile>();
        }
    }
}
