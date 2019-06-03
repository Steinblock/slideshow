using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Models
{
    // https://docs.microsoft.com/de-de/aspnet/core/mvc/models/model-binding?view=aspnetcore-2.2
    public class SlideViewModel
    {
        public int Id { get; set; }
        [StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        public string Header { get; set; }
        public string Template { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public int SectionId { get; set; }
    }

    public class SlideViewModelWithNavigation : SlideViewModel
    {
        public SlideViewModel Prev { get; set; }
        public SlideViewModel Next { get; set; }
    }
}
