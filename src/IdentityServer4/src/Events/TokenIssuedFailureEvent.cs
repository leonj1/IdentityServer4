using IdentityServer4.Extensions;
using IdentityServer4.Validation;
using static IdentityServer4.Constants;

namespace IdentityServer4.Events
{
    /// <summary>
    /// Event for failed token issuance
    /// </summary>
    /// <seealso cref="IdentityServer4.Events.Event" />
    public class TokenIssuedFailureEvent : Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="error">The error.</param>
        /// <param name="description">The description.</param>
        public TokenIssuedFailureEvent(ValidatedAuthorizeRequest request, string error, string description)
            : this()
        {
            if (request != null)
            {
                ClientId = request.ClientId;
                ClientName = request.Client?.ClientName;
                RedirectUri = request.RedirectUri;
                Scopes = request.RequestedScopes?.ToSpaceSeparatedString();
                GrantType = request.GrantType;

                if (request.Subject != null && request.Subject.Identity.IsAuthenticated)
                {
                    SubjectId = request.Subject?.GetSubjectId();
                }
            }

            Endpoint = EndpointNames.Authorize;
            Error = error;
            ErrorDescription = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        /// <param name="result">The result.</param>
        public TokenIssuedFailureEvent(TokenRequestValidationResult result)
            : this()
        {
            if (result != null)
            {
                ClientId = result.Client.ClientId;
                ClientName = result.Client.ClientName;
                RedirectUri = result.RedirectUri;
                Scopes = string.Join(" ", result.ValidatedResources.RawScopeValues);
                GrantType = result.GrantType;

                if (result.Subject != null && result.Subject.Identity.IsAuthenticated)
                {
                    SubjectId = result.Subject?.GetSubjectId();
                }
            }

            Endpoint = EndpointNames.Authorize;
            Error = result.Error;
            ErrorDescription = result.ErrorDescription;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedFailureEvent"/> class.
        /// </summary>
        public TokenIssuedFailureEvent()
        {
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        /// <value>
        /// The client name.
        /// </value>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the redirect URI.
        /// </summary>
        /// <value>
        /// The redirect URI.
        /// </value>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>
        /// The endpoint.
        /// </value>
        public string Endpoint { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the scopes.
        /// </summary>
        /// <value>
        /// The scopes.
        /// </value>
        public string Scopes { get; set; }

        /// <summary>
        /// Gets or sets the grant type.
        /// </summary>
        /// <value>
        /// The grant type.
        /// </value>
        public string GrantType { get; set; }

        /// <summary>
        /// Gets or sets the error.
        /// </summary>
        /// <value>
        /// The error.
        /// </value>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        /// <value>
        /// The error description.
        /// </value>
        public string ErrorDescription { get; set; }
    }
}
