using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILoggerFactory loggerFactory;

        public HomeController(ISectionRepository repo, ILoggerFactory loggerFactory)
        {
            this.repo = repo;
            this.loggerFactory = loggerFactory;
        }

        public IActionResult Index()
        {
            var section = repo.GetAllSections().OrderBy(x => x.Order).FirstOrDefault();

            var sb = new StringBuilder();
            sb.AppendLine("<h2>Jürgen Steinblock</h2>")
                .AppendLine("<h3>Vortrag zum Thema DevOps in der Praxis</h3>")
                .AppendLine($"<h4>24.04.2019 und 26.06.2019</h4>")
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
        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                var logger = loggerFactory.CreateLogger<HomeController>();
                if (statusCode.Value >= 400 && statusCode.Value < 500)
                {
                    logger.LogWarning("StatusCode {0}", statusCode);
                }
                else if (statusCode.Value >= 500 && statusCode.Value < 600)
                {
                    logger.LogError("StatusCode {0}", statusCode);
                }
            }
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error(int? statusCode = null)
        //{
        //    if (statusCode.HasValue)
        //    {
        //        if (statusCode.Value == 404 || statusCode.Value == 500)
        //        {
        //            var viewName = statusCode.ToString();
        //            return View(viewName);
        //        }
        //    }
        //    return View();
        //}

    }
}
