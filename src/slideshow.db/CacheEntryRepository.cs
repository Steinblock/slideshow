using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.data.Models;

namespace slideshow.db
{
    public class CacheEntryRepository : Repository, ICacheEntryRepository
    {
        public CacheEntryRepository(SlideshowContext context) : base(context)
        {
        }

        public ICacheEntry CreateCacheEntry(string key)
        {
            var entry = new CacheEntry();
            entry.Key = key;
            context.Add(entry);
            return entry;
        }

        public void DeleteCacheEntry(ICacheEntry entry)
        {
            this.context.Remove(entry);
        }

        public IEnumerable<ICacheEntry> GetAllCacheEntries()
        {
            return this.context.CacheEntries;
        }

        public ICacheEntry GetCacheEntry(string key)
        {
            return this.context
                .CacheEntries
                .Where(x => x.Key == key)
                .SingleOrDefault();
        }

        public async Task<ICacheEntry> GetCacheEntryAsync(string key, CancellationToken token)
        {
            return await this.context
                .CacheEntries
                .Where(x => x.Key == key)
                .SingleOrDefaultAsync(token);
        }
    }
}
