using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class GetAllEnabledResourcesAsyncOnDuplicateIdentityScopesShouldFailTests
{
    [Fact]
    public async Task GetAllEnabledResourcesAsync_on_duplicate_identity_scopes_should_fail()
    {
        var store = new MockResourceStore();
        store.IdentityResources.Add(new IdentityResource { Name = "identity1" });
        store.IdentityResources.Add(new IdentityResource { Name = "identity2" });

        await Assert.ThrowsAsync<InvalidOperationException>(() => store.GetAllResourcesAsync());
    }
}
