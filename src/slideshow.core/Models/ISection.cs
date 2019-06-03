using System.Collections.Generic;

namespace slideshow.core.Models
{
    public interface ISection
    {
        int SectionId { get; set; }
        string Name { get; set; }
        int Order { get; set; }
        string Class { get; set; }
        IList<ISlide> Slides { get; set; }
    }
}
