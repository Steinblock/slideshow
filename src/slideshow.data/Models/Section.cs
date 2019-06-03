using slideshow.core.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace slideshow.data.Models
{

    public class Section : ISection
    {
        [Key]
        public int SectionId { get; set; }
        [Required, StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        public int Order { get; set; }
        public string Class { get; set; }
        public IList<Slide> Slides { get; set; }

        IList<ISlide> ISection.Slides
        {
            get => (IList<ISlide>)Slides;
            set => Slides = (IList<Slide>)value;
        }
    }
}
