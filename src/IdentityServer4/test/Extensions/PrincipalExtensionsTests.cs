using System;
using System.Security.Claims;
using System.Linq;
using FluentAssertions;
using IdentityModel;
using Xunit;
using IdentityServer4.Extensions;

namespace IdentityServer4.UnitTests.Extensions
{
    public class PrincipalExtensionsTests
    {
        [Fact]
        public void GetAuthenticationTime_Should_Return_Correct_DateTime()
        {
            // Arrange
            var expectedTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Utc);
            var epoch = ((DateTimeOffset)expectedTime).ToUnixTimeSeconds();
            
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(JwtClaimTypes.AuthenticationTime, epoch.ToString())
            });
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetAuthenticationTime();

            // Assert
            result.Should().Be(expectedTime);
        }

        [Fact]
        public void GetSubjectId_Should_Return_Subject_Value()
        {
            // Arrange
            var expectedSubject = "123";
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(JwtClaimTypes.Subject, expectedSubject)
            });
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetSubjectId();

            // Assert
            result.Should().Be(expectedSubject);
        }

        [Fact]
        public void GetSubjectId_Should_Throw_When_Subject_Missing()
        {
            // Arrange
            var identity = new ClaimsIdentity(new Claim[] { });
            var principal = new ClaimsPrincipal(identity);

            // Act & Assert
            Action act = () => principal.GetSubjectId();
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("sub claim is missing");
        }

        [Fact]
        public void GetDisplayName_Should_Return_Name_When_Present()
        {
            // Arrange
            var expectedName = "John Doe";
            var identity = new ClaimsIdentity(new Claim[] 
            {
                new Claim(ClaimTypes.Name, expectedName)
            });
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetDisplayName();

            // Assert
            result.Should().Be(expectedName);
        }

        [Fact]
        public void GetAuthenticationMethods_Should_Return_All_Methods()
        {
            // Arrange
            var methods = new[] { "method1", "method2" };
            var claims = methods.Select(m => new Claim(JwtClaimTypes.AuthenticationMethod, m));
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.GetAuthenticationMethods();

            // Assert
            result.Select(c => c.Value).Should().BeEquivalentTo(methods);
        }

        [Fact]
        public void IsAuthenticated_Should_Return_True_For_Authenticated_Principal()
        {
            // Arrange
            var identity = new ClaimsIdentity(new Claim[] { }, "test");
            var principal = new ClaimsPrincipal(identity);

            // Act
            var result = principal.IsAuthenticated();

            // Assert
            result.Should().BeFalse();
        }
    }
}
