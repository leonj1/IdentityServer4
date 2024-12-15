using IdentityServer4.Configuration;
using IdentityServer4.Extensions;

namespace IdentityServer.UnitTests.Extensions
{
    public class EndpointsOptions
    {
        public bool EnableAuthorizeEndpoint { get; set; }
        public bool EnableCheckSessionEndpoint { get; set; }
        public bool EnableDeviceAuthorizationEndpoint { get; set; }
        public bool EnableDiscoveryEndpoint { get; set; }
        public bool EnableEndSessionEndpoint { get; set; }
        public bool EnableIntrospectionEndpoint { get; set; }
        public bool EnableTokenEndpoint { get; set; }
        public bool EnableTokenRevocationEndpoint { get; set; }
        public bool EnableUserInfoEndpoint { get; set; }

        public bool IsEndpointEnabled(Endpoint endpoint)
        {
            switch (endpoint.Name)
            {
                case EndpointNames.Authorize:
                    return EnableAuthorizeEndpoint;
                case EndpointNames.CheckSession:
                    return EnableCheckSessionEndpoint;
                case EndpointNames.DeviceAuthorization:
                    return EnableDeviceAuthorizationEndpoint;
                case EndpointNames.Discovery:
                    return EnableDiscoveryEndpoint;
                case EndpointNames.EndSession:
                    return EnableEndSessionEndpoint;
                case EndpointNames.Introspection:
                    return EnableIntrospectionEndpoint;
                case EndpointNames.Token:
                    return EnableTokenEndpoint;
                case EndpointNames.Revocation:
                    return EnableTokenRevocationEndpoint;
                case EndpointNames.UserInfo:
                    return EnableUserInfoEndpoint;
                default:
                    return false;
            }
        }

        private Endpoint CreateTestEndpoint(string name)
        {
            return new Endpoint(name, "", null);
        }
    }
}
