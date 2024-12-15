using IdentityServer4.Configuration;

namespace IdentityServer.UnitTests.Extensions
{
    public static class EndpointChecker
    {
        public static bool IsEndpointEnabled(Endpoint endpoint)
        {
            switch (endpoint.Name)
            {
                case EndpointNames.Authorize:
                    return EndpointsOptions.EnableAuthorizeEndpoint;
                case EndpointNames.CheckSession:
                    return EndpointsOptions.EnableCheckSessionEndpoint;
                case EndpointNames.DeviceAuthorization:
                    return EndpointsOptions.EnableDeviceAuthorizationEndpoint;
                case EndpointNames.Discovery:
                    return EndpointsOptions.EnableDiscoveryEndpoint;
                case EndpointNames.EndSession:
                    return EndpointsOptions.EnableEndSessionEndpoint;
                case EndpointNames.Introspection:
                    return EndpointsOptions.EnableIntrospectionEndpoint;
                case EndpointNames.Token:
                    return EndpointsOptions.EnableTokenEndpoint;
                case EndpointNames.Revocation:
                    return EndpointsOptions.EnableTokenRevocationEndpoint;
                case EndpointNames.UserInfo:
                    return EndpointsOptions.EnableUserInfoEndpoint;
                default:
                    return false;
            }
        }
    }
}
