using slideshow.core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace slideshow.data.Models
{
    public class CacheEntry : ICacheEntry
    {
        [Key, Required]
        public string Key { get; set; }
        [Required]  
        public string Value { get; set; }
        public DateTimeOffset? AbsoluteExpiration { get; set; }
        public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; }
        public TimeSpan? SlidingExpiration { get; set; }
    }
}
