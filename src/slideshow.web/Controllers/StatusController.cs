using Microsoft.AspNetCore.Mvc;
using slideshow.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public class StatusController : Controller
    {
        private readonly IEnumerable<IService> services;

        public StatusController(IEnumerable<IService> services)
        {
            this.services = services;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (this.services.Any(x => x.Status != ServiceStatus.Running))
            {
                this.HttpContext.Response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;
            }
            return Negotiate("Index", this.services);
        }

        [HttpPost]
        public async Task<IActionResult> Index([FromForm]string name, [FromForm]string action)
        {

            var service = this.services.Where(x => x.Name == name).SingleOrDefault();

            if (action == "Start")
            {
                await service.StartAsync();
            }
            else if (action == "Stop")
            {
                await service.StopAsync();
            }
            else if (action == "Crash me")
            {
                Environment.Exit(-1);
            }

            return Negotiate("Index", this.services);
        }

    }
}
