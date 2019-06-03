using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Models
{
    public class PagedModel<T>
    {
        public int current { get; set; }
        public int rowCount { get; set; }
        public int total { get; set; }
        public IEnumerable<T> rows { get; set; }
    }
}
