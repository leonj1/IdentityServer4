using IdentityModel;
using System.Linq;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models the standard OpenID scope
    /// </summary>
    /// <seealso cref="IdentityServer4.Models.IdentityResource" />
    public class OpenId : IdentityResource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenId"/> class.
        /// </summary>
        public OpenId()
        {
            Name = IdentityServerConstants.StandardScopes.OpenId;
            DisplayName = "Identity";
            UserClaims = Constants.StandardScopes.OpenId.ToList();
        }
    }
}
