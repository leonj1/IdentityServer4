using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ApiResourceMappersConfigTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            ApiResourceMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ApiResourceMapperProfile>();
        }
    }
}
