using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {

        public IActionResult Negotiate(object model)
        {
            return Negotiate(null, model);
        }

        public IActionResult Negotiate(string viewName, object model)
        {
            var acceptTypes = this.HttpContext == null 
                ? new string[] { "application/json" }
                : this.HttpContext.Request.Headers.GetCommaSeparatedValues("Accept");
            foreach(var acceptType in acceptTypes)
            {
                if (acceptType.StartsWith("text/html", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                else if (acceptType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) ||
                         acceptType.StartsWith("application/xml", StringComparison.OrdinalIgnoreCase)
                ) {
                    if (!this.ModelState.IsValid)
                    {
                        return ValidationProblem();
                    }
                    return Ok(model);
                }

            }

            return View(viewName, model);
        }

        //public override ViewResult View()
        //{
        //    return Negotiate(null, null);
        //}

        //public override ViewResult View(object model)
        //{
        //    return Negotiate(null, model);
        //}

        //public override ViewResult View(string viewName)
        //{
        //    return Negotiate(viewName, null);
        //}

        //public override ViewResult View(string viewName, object model)
        //{
        //    return Negotiate(viewName, model);
        //}
    }
}
