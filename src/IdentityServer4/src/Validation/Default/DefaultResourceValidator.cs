using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    public class DefaultResourceValidator
    {
        /// <summary>
        /// Validates the requested resources.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resources">The resources.</param>
        /// <returns></returns>
        public virtual async Task ValidateResourcesAsync(Client client, Resources resources)
        {
            // Implementation of ValidateResourcesAsync
        }

        /// <summary>
        /// Validates the requested scope.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="resources">The resources.</param>
        /// <param name="requestedScope">The requested scope.</param>
        /// <returns></returns>
        protected virtual async Task ValidateScopeAsync(
            Client client, 
            Resources resources, 
            ParsedScopeValue requestedScope)
        {
            // Implementation of ValidateScopeAsync
        }

        /// <summary>
        /// Determines if client is allowed access to the identity scope.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="identity">The identity.</param>
        /// <returns></returns>
        protected virtual Task<bool> IsClientAllowedIdentityResourceAsync(Client client, IdentityResource identity)
        {
            // Implementation of IsClientAllowedIdentityResourceAsync
        }

        /// <summary>
        /// Determines if client is allowed access to the API scope.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="apiScope">The api scope.</param>
        /// <returns></returns>
        protected virtual Task<bool> IsClientAllowedApiScopeAsync(Client client, ApiScope apiScope)
        {
            // Implementation of IsClientAllowedApiScopeAsync
        }

        /// <summary>
        /// Validates if the client is allowed offline_access.
        /// </summary>
        /// <param name="client">The client.</param>
        /// <returns></returns>
        protected virtual Task<bool> IsClientAllowedOfflineAccessAsync(Client client)
        {
            // Implementation of IsClientAllowedOfflineAccessAsync
        }
    }
}
