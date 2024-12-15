using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ClientMapperProfileTests
    {
        [Fact]
        public void AutomapperConfigurationIsValid()
        {
            ClientMappers.Mapper.ConfigurationProvider.AssertConfigurationIsValid<ClientMapperProfile>();
        }
    }
}
