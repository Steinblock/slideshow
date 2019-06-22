using slideshow.core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.core.Repository
{
    //public interface IDistributedCacheRepository
    //{
    //    string Get(string key);
    //    Task<string> GetAsync(string key, CancellationToken token = default(CancellationToken));
    //    void Refresh(string key);
    //    Task RefreshAsync(string key, CancellationToken token);
    //    void Remove(string key);
    //    Task RemoveAsync(string key, CancellationToken token);
    //    void Set(string key, string base64Value, DateTimeOffset? absoluteExpiration, TimeSpan? absoluteExpirationRelativeToNow, TimeSpan? slidingExpiration);
    //    Task SetAsync(string key, string base64Value, DateTimeOffset? absoluteExpiration, TimeSpan? absoluteExpirationRelativeToNow, TimeSpan? slidingExpiration, CancellationToken token);
    //}

    public interface ICacheEntryRepository : IRepository
    {
        ICacheEntry GetCacheEntry(string key);
        Task<ICacheEntry> GetCacheEntryAsync(string key, CancellationToken token);
        void DeleteCacheEntry(ICacheEntry entry);
        ICacheEntry CreateCacheEntry(string key);
        IEnumerable<ICacheEntry> GetAllCacheEntries();
    }
}
