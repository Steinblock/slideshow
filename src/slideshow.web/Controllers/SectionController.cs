using Microsoft.AspNetCore.Mvc;
using slideshow.core.Models;
using slideshow.core.Repository;
using slideshow.web.Models;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public class SectionController : Controller
    {
        private ISectionRepository repo;

        public SectionController(ISectionRepository repo)
        {
            this.repo = repo;
        }

        public IActionResult Index([FromQuery] int current = 1, [FromQuery] int rowCount = 10, [FromQuery] string searchPhrase = null)
        {

            var query = repo.GetAllSections();

            if (!string.IsNullOrEmpty(searchPhrase))
            {
                query = query.Where(x => x.Name.Contains(searchPhrase, System.StringComparison.OrdinalIgnoreCase));
            }
            var total = query.Count();

            var sections = query.Skip((current - 1) * rowCount).Take(rowCount);

            var model = new PagedModel<SectionViewModel>
            {
                current = current,
                rowCount = rowCount,
                total = total,
                rows = from s in query
                       select CreateSectionViewModel(s)
            };


            //.Select(x =>
            //new SectionViewModel
            //{
            //    SectionId = x.SectionId,
            //    Name = x.Name,
            //});
            return Negotiate(model);
        }

        [HttpGet("/section/create")]
        public async Task<IActionResult> Create()
        {
            ViewData["Title"] = "Create";
            var section = await repo.CreateSectionAsync();
            var model = CreateSectionViewModel(section);
            return Negotiate("Edit", model);
        }

        [HttpGet("/section/{id:int}")]
        public async Task<IActionResult> View(int id)
        {
            ViewData["Title"] = "View";
            var section = await repo.GetSectionAsync(id);
            var model = CreateSectionViewModel(section);
            return Negotiate("View", model);
        }

        [HttpGet("/section/edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            ViewData["Title"] = "Edit";
            var section = await repo.GetSectionAsync(id);
            var model = CreateSectionViewModel(section);
            return Negotiate("Edit", model);
        }

        [HttpPut("/section/")]
        [HttpPost("/section/create")]
        //[HttpPost("/section/{id}")]
        [HttpPost("/section/edit/{id}")]
        public async Task<IActionResult> Edit([FromForm] SectionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var section = model.Id <= 0 ? await repo.CreateSectionAsync() : await repo.GetSectionAsync(model.Id);
                section.Name = model.Name;
                section.Order = model.Order;
                section.Class = model.Class;
                await repo.SaveAsync();
                return RedirectToAction("Edit", new { id = section.SectionId });
            }
            return Negotiate("Edit", model);
        }

        [HttpGet("/section/delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            ViewData["Title"] = "Delete";
            ViewData["Save-Button-Label"] = "Ok";
            ViewData["Close-Button-Label"] = "Cancel";

            var section = await repo.GetSectionAsync(id);
            var model = CreateSectionViewModel(section);
            return Negotiate("Delete", model);
        }

        [HttpDelete("/section/")]
        [HttpPost("/section/delete/{id}")]
        public async Task<IActionResult> Delete([FromForm] SectionViewModel model)
        {

            var section = await repo.GetSectionAsync(model.Id);
            repo.DeleteSection(section);
            await repo.SaveAsync();
            return RedirectToAction("Index");
        }

        [HttpGet("/section/up/{id}")]
        public async Task<IActionResult> Up(int id)
        {

            var sections = repo.GetAllSections().ToList();
            var a = sections.Where(x => x.SectionId == id).Single();
            var b = sections.Where(x => x.Order < a.Order).OrderByDescending(x => x.Order).FirstOrDefault();
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
            foreach (var section in sections.OrderBy(x => x.Order))
            {
                section.Order = order++;
            }

            await repo.SaveAsync();

            return Ok(a);
        }

        [HttpGet("/section/down/{id}")]
        public async Task<IActionResult> Down(int id)
        {

            var sections = repo.GetAllSections().ToList();
            var a = sections.Where(x => x.SectionId == id).Single();
            var b = sections.Where(x => x.Order > a.Order).OrderBy(x => x.Order).FirstOrDefault();
            if (b == null)
            {
                a.Order = sections.Max(x => x.Order) + 1;
            }
            else
            {
                var _order = a.Order;
                a.Order = b.Order;
                b.Order = _order;
            }

            int order = 0;
            foreach (var section in sections.OrderBy(x => x.Order))
            {
                section.Order = order++;
            }

            await repo.SaveAsync();

            return Ok(a);
        }

        private SectionViewModel CreateSectionViewModel(ISection section)
        {
            return new SectionViewModel
            {
                Id = section.SectionId,
                Name = section.Name,
                Class = section.Class,
                Order = section.Order,
            };
        }

    }
}
