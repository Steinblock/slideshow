namespace slideshow.core.Models
{
    public interface ISlide
    {
        int SlideId { get; set; }
        string Name { get; set; }
        string Header { get; set; }
        string Content { get; set; }

        string Template { get; set; }
        int Order { get; set; }

        int SectionId { get; set; }
        ISection Section { get; set; }
    }
}
