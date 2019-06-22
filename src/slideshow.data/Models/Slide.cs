using slideshow.core.Models;
using System.ComponentModel.DataAnnotations;

namespace slideshow.data.Models
{
    public class Slide : ISlide
    {
        [Key]
        public int SlideId { get; set; }
        [Required, StringLength(20, MinimumLength = 3)]
        public string Name { get; set; }
        [Required, StringLength(100, MinimumLength = 3)]
        public string Header { get; set; }
        public string Content { get; set; }

        public string Template { get; set; }
        public int Order { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        ISection ISlide.Section
        {
            get => Section;
            set => Section = (Section)value;
        }
    }
}
