using System;
using System.Collections.Generic;

namespace YourNamespace // Replace with your actual namespace
{
    public class TokenIssuedSuccessEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TokenIssuedSuccessEvent"/> class.
        /// </summary>
        protected TokenIssuedSuccessEvent()
            : base(EventCategories.Token,
                  "Token Issued Success",
                  EventTypes.Success,
                  EventIds.TokenIssuedSuccess)
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
        /// Gets or sets the name of the client.
        /// </summary>
        /// <value>
        /// The name of the client.
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
        /// Gets or sets the tokens.
        /// </summary>
        /// <value>
        /// The tokens.
        /// </value>
        public IEnumerable<Token> Tokens { get; set; }

        /// <summary>
        /// Data structure serializing issued tokens
        /// </summary>
        public class Token
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Token"/> class.
            /// </summary>
            /// <param name="type">The type.</param>
            /// <param name="value">The value.</param>
            public Token(string type, string value)
            {
                TokenType = type;
                TokenValue = Obfuscate(value);
            }

            /// <summary>
            /// Gets the type of the token.
            /// </summary>
            /// <value>
            /// The type of the token.
            /// </value>
            public string TokenType { get; }

            /// <summary>
            /// Gets the token value.
            /// </summary>
            /// <value>
            /// The token value.
            /// </value>
            public string TokenValue { get; }
        }
    }
}
