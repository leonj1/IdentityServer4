// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Validation;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace IdentityServer4.Models
{
    /// <summary>
    /// Models the entire parameter collection.
    /// </summary>
    public class LogoutMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogoutMessage"/> class.
        /// </summary>
        /// <param name="clientId">The client identifier.</param>
        /// <param name="clientName">The client name.</param>
        /// <param name="postLogoutRedirectUri">The post logout redirect URI.</param>
        /// <param name="subjectId">The subject identifier for the user at logout time.</param>
        /// <param name="sessionId">The session identifier for the user at logout time.</param>
        /// <param name="clientIds">Ids of clients known to have an authentication session for user at end session time</param>
        public LogoutMessage(string clientId, string clientName, string postLogoutRedirectUri, string subjectId, string sessionId, IEnumerable<string> clientIds)
        {
            ClientId = clientId;
            ClientName = clientName;
            PostLogoutRedirectUri = postLogoutRedirectUri;
            SubjectId = subjectId;
            SessionId = sessionId;
            ClientIds = clientIds?.ToList();
        }

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client name.
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// Gets or sets the post logout redirect URI.
        /// </summary>
        public string PostLogoutRedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the subject identifier for the user at logout time.
        /// </summary>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets or sets the session identifier for the user at logout time.
        /// </summary>
        public string SessionId { get; set; }

        /// <summary>
        ///  Ids of clients known to have an authentication session for user at end session time
        /// </summary>
        public IEnumerable<string> ClientIds { get; set; }

        /// <summary>
        /// Gets the entire parameter collection.
        /// </summary>
        public IDictionary<string, string[]> Parameters { get; set; } = new Dictionary<string, string[]>();

        /// <summary>
        ///  Flag to indicate if the payload contains useful information or not to avoid serailization.
        /// </summary>
        internal bool ContainsPayload => ClientId.IsPresent() || ClientIds?.Any() == true;
    }
}
