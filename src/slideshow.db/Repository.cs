using slideshow.core.Repository;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.db
{
    public abstract class Repository : IRepository
    {
        protected readonly SlideshowContext context;

        public Repository(SlideshowContext context)
        {
            this.context = context;
        }

        public async Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return await context.SaveChangesAsync();
        }

        public int Save()
        {
            return context.SaveChanges();
        }
    }
}
