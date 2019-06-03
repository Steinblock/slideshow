using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.core.Repository
{
    public interface IRepository
    {
        Task<int> SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}
