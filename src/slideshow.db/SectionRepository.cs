using Microsoft.EntityFrameworkCore;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.db
{
    public class SectionRepository : Repository, ISectionRepository
    {
        public SectionRepository(SlideshowContext context) : base(context)
        {
        }

        public async Task<ISection> CreateSectionAsync()
        {
            var section = new Section();
            section.Order = await context.Sections.CountAsync() > 0 ? await context.Sections.MaxAsync(x => x.Order) + 1 : 0;
            context.Add(section);
            return section;
        }

        public async Task<ISection> GetSectionAsync(int id)
        {
            return await this.context
                .Sections
                .Where(x => x.SectionId == id)
                .SingleOrDefaultAsync();
        }

        public void DeleteSection(ISection section)
        {
            this.context.Remove(section);
        }

        public IQueryable<ISection> GetAllSections()
        {
            return context.Sections.OrderBy(x => x.Order);
        }

        public IQueryable<ISlide> GetSlides(ISection section)
        {
            return context.Slides.Where(x => x.Section == section).OrderBy(x => x.Order);
        }

        public void AddSection(ISection section)
        {
            var _section = section as Section ?? throw new NotSupportedException();
            context.Sections.Add(_section);
        }
    }
}
