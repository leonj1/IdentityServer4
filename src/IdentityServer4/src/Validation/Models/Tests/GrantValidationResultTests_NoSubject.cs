using System.Security.Claims;
using IdentityModel;
using IdentityServer4.Validation;
using Xunit;

namespace IdentityServer4.UnitTests.Validation
{
    public class GrantValidationResultNoSubjectTests
    {
        [Fact]
        public void Constructor_WithNoSubject_ShouldNotBeError()
        {
            var result = new GrantValidationResult();
            
            Assert.False(result.IsError);
            Assert.Null(result.Subject);
            Assert.Empty(result.CustomResponse);
        }
    }
}
