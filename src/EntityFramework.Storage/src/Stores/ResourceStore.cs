using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YourNamespace // Replace with your actual namespace
{
    public class ResourceStore
    {
        private readonly ApplicationDbContext _context; // Assuming you have an ApplicationDbContext

        public ResourceStore(ApplicationDbContext context)
        {
            _context = context;
        }

        public virtual async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var names = scopeNames.ToArray();

            var query =
                from api in _context.ApiResources
                where api.Scopes.Where(x => names.Contains(x.Scope)).Any()
                select api;

            var apis = query
                .Include(x => x.Secrets)
                .Include(x => x.Scopes)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = (await apis.ToArrayAsync())
                .Where(api => api.Scopes.Any(x => names.Contains(x.Scope)));
            var models = results.Select(x => x.ToModel()).ToArray();

            // Logger.LogDebug("Found {apis} API resources in database", models.Select(x => x.Name));

            return models;
        }

        public virtual async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from identityResource in _context.IdentityResources
                where scopes.Contains(identityResource.Name)
                select identityResource;

            var resources = query
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = (await resources.ToArrayAsync())
                .Where(x => scopes.Contains(x.Name));

            // Logger.LogDebug("Found {scopes} identity scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        public virtual async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var scopes = scopeNames.ToArray();

            var query =
                from scope in _context.ApiScopes
                where scopes.Contains(scope.Name)
                select scope;

            var resources = query
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var results = (await resources.ToArrayAsync())
                .Where(x => scopes.Contains(x.Name));

            // Logger.LogDebug("Found {scopes} scopes in database", results.Select(x => x.Name));

            return results.Select(x => x.ToModel()).ToArray();
        }

        public virtual async Task<Resources> GetAllResourcesAsync()
        {
            var identity = _context.IdentityResources
              .Include(x => x.UserClaims)
              .Include(x => x.Properties)
              .AsNoTracking();
            
            var apis = _context.ApiResources
                .Include(x => x.Secrets)
                .Include(x => x.Scopes)
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();
            
            var scopes = _context.ApiScopes
                .Include(x => x.UserClaims)
                .Include(x => x.Properties)
                .AsNoTracking();

            var result = new Resources(
                (await identity.ToArrayAsync()).Select(x => x.ToModel()),
                (await apis.ToArrayAsync()).Select(x => x.ToModel()),
                (await scopes.ToArrayAsync()).Select(x => x.ToModel())
            );

            // Logger.LogDebug("Found {scopes} as all scopes, and {apis} as API resources", 
            //     result.IdentityResources.Select(x=>x.Name).Union(result.ApiScopes.Select(x=>x.Name)),
            //     result.ApiResources.Select(x=>x.Name));

            return result;
        }
    }
}
