// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using IdentityServer4.Stores;
using IdentityServer4.Stores.Serialization;
using System.Collections.Generic;
using System.Linq;
using System;

namespace IdentityServer4.Services
{
    /// <summary>
    /// Default persisted grant service
    /// </summary>
    public class DefaultPersistedGrantService : IPersistedGrantService
    {
        private readonly ILogger _logger;
        private readonly IPersistedGrantStore _store;
        private readonly IPersistentGrantSerializer _serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPersistedGrantService"/> class.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="serializer">The serializer.</param>
        /// <param name="logger">The logger.</param>
        public DefaultPersistedGrantService(IPersistedGrantStore store, IPersistentGrantSerializer serializer, ILogger<DefaultPersistedGrantService> logger)
        {
            _store = store;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Grant>> GetAllGrantsAsync(string subjectId)
        {
            if (String.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));

            var grants = await _store.GetAllAsync(new PersistedGrantFilter { SubjectId = subjectId });

            try
            {
                var consents = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.Consent).Select(x => _serializer.Deserialize<Consent>(x.Data));
                var codes = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.AuthorizationCode).Select(x => _serializer.Deserialize<AuthorizationCode>(x.Data));
                var refreshTokens = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.RefreshToken).Select(x => _serializer.Deserialize<Token>(x.Data));
                var referenceTokens = grants.Where(x => x.Type == IdentityServerConstants.PersistedGrantTypes.ReferenceToken).Select(x => _serializer.Deserialize<Token>(x.Data));

                return Join(consents, codes, refreshTokens, referenceTokens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining grants");
                throw;
            }
        }

        private IEnumerable<Grant> Join(IEnumerable<Grant> consents, IEnumerable<Grant> codes, IEnumerable<Grant> refreshTokens, IEnumerable<Grant> referenceTokens)
        {
            var list = new List<Grant>();

            foreach (var grant in consents.Concat(codes).Concat(refreshTokens).Concat(referenceTokens))
            {
                var match = list.FirstOrDefault(x => x.ClientId == grant.ClientId);
                if (match != null)
                {
                    match.Scopes = match.Scopes.Union(grant.Scopes).Distinct();

                    if (match.CreationTime > grant.CreationTime)
                    {
                        // show the earlier creation time
                        match.CreationTime = grant.CreationTime;
                    }

                    if (match.Expiration == null || grant.Expiration == null)
                    {
                        // show that there is no expiration to one of the grants
                        match.Expiration = null;
                    }
                    else if (match.Expiration < grant.Expiration)
                    {
                        // show the latest expiration
                        match.Expiration = grant.Expiration;
                    }

                    match.Description = match.Description ?? grant.Description;
                }
                else
                {
                    list.Add(grant);
                }
            }

            return list;
        }

        /// <inheritdoc/>
        public Task RemoveAllGrantsAsync(string subjectId, string clientId = null, string sessionId = null)
        {
            if (String.IsNullOrWhiteSpace(subjectId)) throw new ArgumentNullException(nameof(subjectId));

            return _store.RemoveAllAsync(new PersistedGrantFilter {
                SubjectId = subjectId,
                ClientId = clientId,
                SessionId = sessionId
            });
        }
    }
}
