using Microsoft.AspNetCore.Mvc;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public class SlideController : Controller
    {
        private readonly ISlideRepository repo;
        private readonly ISectionRepository sectionRepo;

        public SlideController(ISlideRepository repo, ISectionRepository sectionRepo)
        {
            this.repo = repo;
            this.sectionRepo = sectionRepo;
        }

        [HttpGet("/section/{sectionId}/slide")]
        public async Task<IActionResult> Index(int sectionId, [FromQuery] int current = 1, [FromQuery] int rowCount = 10, [FromQuery] string searchPhrase = null)
        {
            var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
       
            var query = repo.GetAllSlides(section);

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(x => x.Name.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase) || x.Header.Contains(searchPhrase, StringComparison.OrdinalIgnoreCase));
            }
            var total = query.Count();

            var sections = query.Skip((current - 1) * rowCount).Take(rowCount);

            var model = new PagedModel<SlideViewModel>
            {
                current = current,
                rowCount = rowCount,
                total = total,
                rows = from s in query
                       select CreateSlideViewModel(s)
            };


            //.Select(x =>
            //new SectionViewModel
            //{
            //    SectionId = x.SectionId,
            //    Name = x.Name,
            //});
            return Negotiate(model);
        }

        [HttpGet("/section/{sectionId}/slide/create")]
        public async Task<IActionResult> Create(int sectionId)
        {
            ViewData["Title"] = "Create";
            var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
            var slide = await repo.CreateSlideAsync(section);
            var model = CreateSlideViewModel(slide);
            return Negotiate("Edit", model);
        }

        [HttpGet("/section/{sectionId}/slide/{id:int}")]
        public async Task<IActionResult> View(int id)
        {
            var slide = await repo.GetSlideAsync(id);
            if (slide == null) return NotFound();
            var prev = await repo.GetPrevSlideAsync(slide);
            var next = await repo.GetNextSlideAsync(slide);
            ViewData["Title"] = slide.Header;
            var model = CreateSlideViewModelWithNavigation(slide, prev, next);
            var template = String.IsNullOrWhiteSpace(model.Template) ? "Default" : model.Template;
            return Negotiate("../Slideshow/" + template, model);
        }
        //[HttpGet("/section/{sectionId}/slide/{id}")]
        [HttpGet("/section/{sectionId}/slide/edit/{id}")]
        public async Task<IActionResult> Edit(int sectionId, int id)
        {
            ViewData["Title"] = "Edit";
            var slide = await repo.GetSlideAsync(id);
            var model = CreateSlideViewModel(slide);
            return Negotiate("Edit", model);
        }

        [HttpPut("/section/{sectionId}/slide/")]
        [HttpPost("/section/{sectionId}/slide/create")]
        //[HttpPost("/section/{sectionId}/slide/{id}")]
        [HttpPost("/section/{sectionId}/slide/edit/{id}")]
        public async Task<IActionResult> Edit(int sectionId, [FromForm] SlideViewModel model)
        {
            if (ModelState.IsValid)
            {
                var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
                var slide = model.Id <= 0 ? await repo.CreateSlideAsync(section) : await repo.GetSlideAsync(model.Id);
                slide.Name = model.Name;
                slide.Header = model.Header;
                slide.Content = model.Content;
                slide.Template = model.Template;
                slide.Order = model.Order;
                await repo.SaveAsync();
                return RedirectToAction("View", new { id = slide.SlideId });
            }
            return Negotiate("Edit", model);
        }

        [HttpGet("/section/{sectionId}/slide/delete/{id}")]
        public async Task<IActionResult> Delete(int sectionId, int id)
        {
            ViewData["Title"] = "Delete";
            ViewData["Save-Button-Label"] = "Ok";
            ViewData["Close-Button-Label"] = "Cancel";
            var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
            var slide = await repo.GetSlideAsync(id);
            var model = CreateSlideViewModel(slide);
            return Negotiate("Delete", model);
        }

        [HttpDelete("/section/{sectionId}/slide/")]
        [HttpPost("/section/{sectionId}/slide/delete/{id}")]
        public async Task<IActionResult> Delete(int sectionId, [FromForm] SlideViewModel model)
        {
            var slide = await repo.GetSlideAsync(model.Id);
            repo.DeleteSlide(slide);
            await repo.SaveAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("/section/{sectionId}/slide/up/{id}")]
        public async Task<IActionResult> Up(int sectionId, int id)
        {
            var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
            var slides = repo.GetAllSlides(section).ToList();
            var a = slides.Where(x => x.SlideId == id).Single();
            var b = slides.Where(x => x.Order < a.Order).OrderByDescending(x => x.Order).FirstOrDefault();
            if (b == null)
            {
                a.Order = 0;
            }
            else
            {
                var _order = a.Order;
                a.Order = b.Order;
                b.Order = _order;
            }

            int order = 0;
            foreach (var slide in slides.OrderBy(x => x.Order))
            {
                slide.Order = order++;
            }

            await repo.SaveAsync();

            return Ok(a);
        }

        [HttpGet("/section/{sectionId}/slide/down/{id}")]
        public async Task<IActionResult> Down(int sectionId, int id)
        {
            var section = await sectionRepo.GetSectionAsync(sectionId) ?? throw new ArgumentNullException("section");
            var slides = repo.GetAllSlides(section).ToList();
            var a = slides.Where(x => x.SlideId == id).Single();
            var b = slides.Where(x => x.Order > a.Order).OrderBy(x => x.Order).FirstOrDefault();
            if (b == null)
            {
                a.Order = slides.Max(x => x.Order) + 1;
            }
            else
            {
                var _order = a.Order;
                a.Order = b.Order;
                b.Order = _order;
            }

            int order = 0;
            foreach (var slide in slides.OrderBy(x => x.Order))
            {
                slide.Order = order++;
            }

            await repo.SaveAsync();

            return Ok(a);
        }

        private SlideViewModel CreateSlideViewModel(ISlide slide)
        {
            return new SlideViewModel
            {
                Id = slide.SlideId,
                Name = slide.Name,
                Header = slide.Header,
                Template = slide.Template,
                Content = slide.Content,
                Order = slide.Order,
                SectionId = slide.SectionId,
                SectionName = slide.Section?.Name,
            };
        }

        private SlideViewModelWithNavigation CreateSlideViewModelWithNavigation(ISlide slide, ISlide prev, ISlide next)
        {
            return new SlideViewModelWithNavigation
            {
                Id = slide.SlideId,
                Name = slide.Name,
                Header = slide.Header,
                Template = slide.Template,
                Content = slide.Content,
                Order = slide.Order,
                SectionId = slide.SectionId,
                SectionName = slide.Section?.Name,
                Prev = prev != null ? CreateSlideViewModel(prev) : null,
                Next = next != null ? CreateSlideViewModel(next) : null,
            };
        }
    }
}
