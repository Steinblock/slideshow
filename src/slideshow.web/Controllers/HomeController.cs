using Microsoft.AspNetCore.Mvc;
using slideshow.web.Models;
using System.Diagnostics;

namespace slideshow.web.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        public IActionResult Index()
        {
            var model = new SlideViewModelWithNavigation
            {
                Name = "Index",
                Header = "Das Hauptproblem ist: Es muss schneller gehen",
                Content = @"
<h2>Jürgen Steinblock</h2>
<h3>24.04.2019</h3>

<p>
<a type=""button"" class=""btn btn-primary"" href=""/section/2/slide/2"">Start</a>
</ p>",
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
