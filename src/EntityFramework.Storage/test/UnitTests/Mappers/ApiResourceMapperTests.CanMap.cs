using System.Linq;
using FluentAssertions;
using IdentityServer4.EntityFramework.Mappers;
using Xunit;

namespace IdentityServer4.EntityFramework.UnitTests.Mappers
{
    public class ApiResourceMappersTests_CanMap
    {
        [Fact]
        public void Can_Map()
        {
            var model = new ApiResource();
            var mappedEntity = model.ToEntity();
            var mappedModel = mappedEntity.ToModel();

            Assert.NotNull(mappedModel);
            Assert.NotNull(mappedEntity);
        }
    }
}
