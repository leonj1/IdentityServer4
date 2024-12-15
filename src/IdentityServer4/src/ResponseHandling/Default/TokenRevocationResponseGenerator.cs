using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.ResponseHandling
{
    /// <summary>
    /// Default revocation response generator
    /// </summary>
    /// <seealso cref="IdentityServer4.ResponseHandling.ITokenRevocationResponseGenerator" />
    public class TokenRevocationResponseGenerator : ITokenRevocationResponseGenerator
    {
        /// <summary>
        /// Gets the reference token store.
        /// </summary>
        /// <value>
        /// The reference token store.
        /// </value>
        protected readonly IReferenceTokenStore ReferenceTokenStore;

        /// <summary>
        /// Gets the refresh token store.
        /// </summary>
        /// <value>
        /// The refresh token store.
        /// </value>
        protected readonly IRefreshTokenStore RefreshTokenStore;

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        protected readonly ILogger Logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenRevocationResponseGenerator" /> class.
        /// </summary>
        /// <param name="referenceTokenStore">The reference token store.</param>
        /// <param name="refreshTokenStore">The refresh token store.</param>
        /// <param name="logger">The logger.</param>
        public TokenRevocationResponseGenerator(IReferenceTokenStore referenceTokenStore, IRefreshTokenStore refreshTokenStore, ILogger<TokenRevocationResponseGenerator> logger)
        {
            ReferenceTokenStore = referenceTokenStore;
            RefreshTokenStore = refreshTokenStore;
            Logger = logger;
        }

        /// <summary>
        /// Revoke access token only if it belongs to client doing the request.
        /// </summary>
        protected virtual async Task<bool> RevokeAccessTokenAsync(TokenRevocationRequestValidationResult validationResult)
        {
            var token = await ReferenceTokenStore.GetReferenceTokenAsync(validationResult.Token);

            if (token != null)
            {
                if (token.ClientId == validationResult.Client.ClientId)
                {
                    Logger.LogDebug("Access token revoked");
                    await ReferenceTokenStore.RemoveReferenceTokenAsync(validationResult.Token);
                }
                else
                {
                    Logger.LogWarning("Client {clientId} denied from revoking access token belonging to Client {tokenClientId}", validationResult.Client.ClientId, token.ClientId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Revoke refresh token only if it belongs to client doing the request.
        /// </summary>
        protected virtual async Task<bool> RevokeRefreshTokenAsync(TokenRevocationRequestValidationResult validationResult)
        {
            var token = await RefreshTokenStore.GetRefreshTokenAsync(validationResult.Token);

            if (token != null)
            {
                if (token.ClientId == validationResult.Client.ClientId)
                {
                    Logger.LogDebug("Refresh token revoked");
                    await RefreshTokenStore.RemoveRefreshTokenAsync(validationResult.Token);
                    await ReferenceTokenStore.RemoveReferenceTokensAsync(token.SubjectId, token.ClientId);
                }
                else
                {
                    Logger.LogWarning("Client {clientId} denied from revoking a refresh token belonging to Client {tokenClientId}", validationResult.Client.ClientId, token.ClientId);
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Process the token revocation request.
        /// </summary>
        public async Task<TokenRevocationResponse> ProcessTokenRevocationAsync(TokenRevocationRequestValidationResult validationResult)
        {
            var response = new TokenRevocationResponse();

            if (validationResult.TokenType == "access_token")
            {
                response.Success = await RevokeAccessTokenAsync(validationResult);
            }
            else if (validationResult.TokenType == "refresh_token")
            {
                response.Success = await RevokeRefreshTokenAsync(validationResult);
            }

            return response;
        }
    }
}
