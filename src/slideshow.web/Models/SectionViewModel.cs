using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Models
{
    // https://docs.microsoft.com/de-de/aspnet/core/mvc/models/model-binding?view=aspnetcore-2.2
    public class SectionViewModel
    {
        public int Id { get; set; }
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        public string Class { get; set; }
        public int Order { get; set; }
    }
}
