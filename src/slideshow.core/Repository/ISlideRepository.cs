using slideshow.core.Models;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.core.Repository
{
    public interface ISlideRepository : IRepository
    {
        IQueryable<ISlide> GetAllSlides(ISection section);
        Task<ISlide> CreateSlideAsync(ISection section);
        Task<ISlide> GetSlideAsync(int id);
        void DeleteSlide(ISlide slide);
        Task<ISlide> GetPrevSlideAsync(ISlide slide);
        Task<ISlide> GetNextSlideAsync(ISlide slide);
    }
}
