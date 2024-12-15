// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using IdentityServer4.Extensions;

namespace IdentityServer4.EntityFramework.Stores
{
    /// <summary>
    /// Implementation of IPersistedGrantStore for Entity Framework.
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore
    {
        private readonly IPersistedGrantDbContext _context;
        private readonly ILogger<PersistedGrantStore> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistedGrantStore"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="logger">The logger.</param>
        public PersistedGrantStore(IPersistedGrantDbContext context, ILogger<PersistedGrantStore> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Stores the grant.
        /// </summary>
        /// <param name="grant">The grant.</param>
        /// <returns></returns>
        public async Task StoreAsync(PersistedGrant grant)
        {
            var persistedGrant = await _context.PersistedGrants
                .Where(x => x.Key == grant.Key)
                .SingleOrDefaultAsync();

            if (persistedGrant != null)
            {
                _context.PersistedGrants.Remove(persistedGrant);
            }

            _context.PersistedGrants.Add(grant.ToEntity());
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the grant.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public async Task RemoveAsync(string key)
        {
            var persistedGrant = await _context.PersistedGrants
                .Where(x => x.Key == key)
                .SingleOrDefaultAsync();

            if (persistedGrant != null)
            {
                _context.PersistedGrants.Remove(persistedGrant);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Gets the grant.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public async Task<PersistedGrant> GetAsync(string key)
        {
            var persistedGrant = await _context.PersistedGrants
                .Where(x => x.Key == key)
                .SingleOrDefaultAsync();

            if (persistedGrant != null)
            {
                return persistedGrant.ToModel();
            }

            return null;
        }

        /// <summary>
        /// Gets the grants.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(PersistedGrantFilter filter)
        {
            var persistedGrants = await Filter(_context.PersistedGrants.AsQueryable(), filter).ToArrayAsync();
            return persistedGrants.Select(x => x.ToModel());
        }

        /// <summary>
        /// Removes the grants.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns></returns>
        public async Task RemoveAllAsync(PersistedGrantFilter filter)
        {
            var persistedGrants = await Filter(_context.PersistedGrants.AsQueryable(), filter).ToArrayAsync();
            _context.PersistedGrants.RemoveRange(persistedGrants);
            await _context.SaveChangesAsync();
        }

        private IQueryable<Entities.PersistedGrant> Filter(IQueryable<Entities.PersistedGrant> query, PersistedGrantFilter filter)
        {
            if (!String.IsNullOrWhiteSpace(filter.ClientId))
            {
                query = query.Where(x => x.ClientId == filter.ClientId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SessionId))
            {
                query = query.Where(x => x.SessionId == filter.SessionId);
            }
            if (!String.IsNullOrWhiteSpace(filter.SubjectId))
            {
                query = query.Where(x => x.SubjectId == filter.SubjectId);
            }
            if (!String.IsNullOrWhiteSpace(filter.Type))
            {
                query = query.Where(x => x.Type == filter.Type);
            }

            return query;
        }
    }
}
