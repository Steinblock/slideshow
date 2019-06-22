using Microsoft.AspNetCore.Mvc;
using slideshow.core.Repository;
using slideshow.web.Models;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace slideshow.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISectionRepository repo;

        public HomeController(ISectionRepository repo)
        {
            this.repo = repo;
        }

        public IActionResult Index()
        {
            var section = repo.GetAllSections().OrderBy(x => x.Order).FirstOrDefault();

            var sb = new StringBuilder();
            sb.AppendLine("<h2>Jürgen Steinblock</h2>")
                .AppendLine($"<h3>{DateTime.Now:g}</h3>")
                .AppendLine();

            if (section != null)
            {
                var slide = repo.GetSlides(section)?.OrderBy(x => x.Order).FirstOrDefault();
                if (slide != null)
                {
                    sb.AppendLine($"<p><a type=\"button\" class=\"btn btn-primary\" href=\"/section/{section.SectionId}/slide/{slide.SlideId}\" >Start</a></p>");
                }
            }

            var model = new SlideViewModelWithNavigation
            {
                Name = "Index",
                Header = "Das Hauptproblem ist: Es muss schneller gehen",
                Content = sb.ToString(),
            };
            return View("../Slideshow/Default", model);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
