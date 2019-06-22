using slideshow.core;
using slideshow.core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow.core.Repository
{
    public interface ISectionRepository : IRepository
    {
        IQueryable<ISection> GetAllSections();
        Task<ISection> CreateSectionAsync();
        Task<ISection> GetSectionAsync(int id);
        IQueryable<ISlide> GetSlides(ISection section);
        void DeleteSection(ISection section);
        void AddSection(ISection section);
    }
}
