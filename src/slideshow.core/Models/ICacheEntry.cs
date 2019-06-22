using System;
using System.Collections.Generic;
using System.Text;

namespace slideshow.core.Models
{
    public interface ICacheEntry
    {
        string Key { get; set; }
        string Value { get; set; }
        DateTimeOffset? AbsoluteExpiration { get; set; }
        TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        TimeSpan? SlidingExpiration { get; set; }
    }
}
