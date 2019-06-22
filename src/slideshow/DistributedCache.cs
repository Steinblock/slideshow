using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow
{
    public class DistributedCache : IDistributedCache
    {
        private readonly ICacheEntryRepository repo;

        public DistributedCache(ICacheEntryRepository repo)
        {
            this.repo = repo;
        }

        public byte[] Get(string key)
        {
            var value = repo.GetCacheEntry(key);
            if (value == null) return null;

            return Convert.FromBase64String(value.Value);
        }

        public async Task<byte[]> GetAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var value = await repo.GetCacheEntryAsync(key, token);
            if (value == null) return null;
            return Convert.FromBase64String(value.Value);
        }

        public void Refresh(string key)
        {
            var value = repo.GetCacheEntry(key);
            if (value != null)
            {
                // ???
            }
        }

        public async Task RefreshAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var value = await repo.GetCacheEntryAsync(key, token);
            if (value != null)
            {
                // ???
            }
            //return repo.RefreshAsync(key, token);
        }

        public void Remove(string key)
        {
            var value = repo.GetCacheEntry(key);
            if (value != null)
            {
                repo.DeleteCacheEntry(value);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken token = default(CancellationToken))
        {
            var value = await repo.GetCacheEntryAsync(key, token);
            if (value != null)
            {
                repo.DeleteCacheEntry(value);
            }
            //return repo.RemoveAsync(key, token);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            // DateTimeOffset AbsoluteExpiration
            // Gets or sets an absolute expiration date for the cache entry.
            // TimeSpan AbsoluteExpirationRelativeToNow
            // Gets or sets an absolute expiration time, relative to now.
            // TimeSpan SlidingExpiration
            // Gets or sets how long a cache entry can be inactive(e.g.not accessed) before it will be removed.This will not extend the entry lifetime beyond the absolute expiration(if set).
            var entry = repo.GetCacheEntry(key) ?? repo.CreateCacheEntry(key);
            entry.Value = Convert.ToBase64String(value);
            entry.AbsoluteExpiration = options.AbsoluteExpiration;
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
            entry.SlidingExpiration = options.SlidingExpiration;

            repo.Save();

        }

        public async Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default(CancellationToken))
        {
            var entry = await repo.GetCacheEntryAsync(key, token) ?? repo.CreateCacheEntry(key);
            entry.Value = Convert.ToBase64String(value);
            entry.AbsoluteExpiration = options.AbsoluteExpiration;
            entry.AbsoluteExpirationRelativeToNow = options.AbsoluteExpirationRelativeToNow;
            entry.SlidingExpiration = options.SlidingExpiration;

            await repo.SaveAsync(token);
        }
    }

    public static class CachingServicesExtensions
    {


        public static IServiceCollection AddDistributedCache(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException("services");
            }
            OptionsServiceCollectionExtensions.AddOptions(services);
            ((ICollection<ServiceDescriptor>)services).Add(ServiceDescriptor.Singleton<IDistributedCache, DistributedCache>());

            return services;
        }
    }
}
