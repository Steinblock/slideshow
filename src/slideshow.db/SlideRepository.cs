using Microsoft.EntityFrameworkCore;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.data.Models;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.db
{
    public class SlideRepository : Repository, ISlideRepository
    {
        public SlideRepository(SlideshowContext context) : base(context)
        {
        }

        public async Task<ISlide> CreateSlideAsync(ISection section)
        {
            var slide = new Slide();
            slide.Order = await context.Slides.Where(x => x.SectionId == section.SectionId).CountAsync() > 0 ? await context.Slides.Where(x => x.SectionId == section.SectionId).MaxAsync(x => x.Order) + 1 : 0;
            slide.SectionId = section.SectionId;
            context.Add(slide);
            return slide;
        }

        public async Task<ISlide> GetSlideAsync(int id)
        {
            return await this.context
                .Slides
                .Where(x => x.SlideId == id)
                .SingleOrDefaultAsync();
        }

        public void DeleteSlide(ISlide slide)
        {
            this.context.Remove(slide);
        }

        public IQueryable<ISlide> GetAllSlides(ISection section)
        {
            return context.Slides.Where(x => x.SectionId == section.SectionId).OrderBy(x => x.Order);
        }

        public async Task<ISlide> GetPrevSlideAsync(ISlide slide)
        {
            var prev = await context.Slides.Where(x => x.SectionId == slide.SectionId && x.Order < slide.Order).OrderByDescending(x => x.Order).FirstOrDefaultAsync();
            if (prev == null)
            {
                var section = await context.Sections.Where(x => x.SectionId == slide.SectionId).SingleAsync();
                var prevSection = await context.Sections.Where(x => x.Order < section.Order).OrderByDescending(x => x.Order).FirstOrDefaultAsync();
                if (prevSection == null) return null;
                prev = await context.Slides.Where(x => x.SectionId == prevSection.SectionId).OrderByDescending(x => x.Order).FirstOrDefaultAsync();
            }
            return prev;
        }

        public async Task<ISlide> GetNextSlideAsync(ISlide slide)
        {
            var next = await context.Slides.Where(x => x.SectionId == slide.SectionId && x.Order > slide.Order).OrderBy(x => x.Order).FirstOrDefaultAsync();
            if (next == null)
            {
                var section = await context.Sections.Where(x => x.SectionId == slide.SectionId).SingleAsync();
                var nextSection = await context.Sections.Where(x => x.Order > section.Order).OrderBy(x => x.Order).FirstOrDefaultAsync();
                if (nextSection == null) return null;
                next = await context.Slides.Where(x => x.SectionId == nextSection.SectionId).OrderBy(x => x.Order).FirstOrDefaultAsync();
            }
            return next;
        }
    }
}
