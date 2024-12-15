using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IdentityServer4.Validation
{
    /// <summary>
    /// Implementation of strict redirect URI validator that allows a random port if 127.0.0.1 is used.
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.StrictRedirectUriValidator" />
    public class StrictRedirectUriValidatorAppAuth : StrictRedirectUriValidator
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="StrictRedirectUriValidatorAppAuth"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public StrictRedirectUriValidatorAppAuth(ILogger<StrictRedirectUriValidatorAppAuth> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Determines whether a redirect URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// <c>true</c> is the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public override async Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (string.IsNullOrEmpty(requestedUri))
                return false;

            if (!requestedUri.StartsWith("http://127.0.0.1", StringComparison.OrdinalIgnoreCase))
                return false;

            var portIndex = requestedUri.IndexOf(':');
            if (portIndex == -1 || portIndex + 1 >= requestedUri.Length)
                return false;

            int port;
            if (!int.TryParse(requestedUri.Substring(portIndex + 1), out port) || port < 0 || port > 65535)
                return false;

            return true;
        }

        /// <summary>
        /// Determines whether a post logout redirect URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// <c>true</c> if the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public override async Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            return await IsRedirectUriValidAsync(requestedUri, client);
        }

        /// <summary>
        /// Determines whether a logout URI is valid for a client.
        /// </summary>
        /// <param name="requestedUri">The requested URI.</param>
        /// <param name="client">The client.</param>
        /// <returns>
        /// <c>true</c> if the URI is valid; <c>false</c> otherwise.
        /// </returns>
        public override async Task<bool> IsLogoutUriValidAsync(string requestedUri, Client client)
        {
            return await IsRedirectUriValidAsync(requestedUri, client);
        }
    }
}
