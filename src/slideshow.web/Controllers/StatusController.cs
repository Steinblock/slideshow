using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using slideshow.core;
using slideshow.core.Models;
using slideshow.core.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public class StatusController : Controller
    {
        private readonly IEnumerable<IService> services;
        private readonly IBackupProvider backupProvider;

        public StatusController(IEnumerable<IService> services, IBackupProvider backupProvider)
        {
            this.services = services;
            this.backupProvider = backupProvider;
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

        public IActionResult Backup()
        {
            var json = backupProvider.Backup();
            return new FileContentResult(Encoding.UTF8.GetBytes(json), "application/json") { FileDownloadName = $"Backup {DateTime.Now}.json" };
        }

        public IActionResult Restore(IFormFile file)
        {
            using (var stream = file.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                var json = reader.ReadToEnd();
                backupProvider.Restore(json);
            }

            return Redirect("~/");
        }
    }
}
