using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using slideshow.web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace slideshow.web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager userManager;

        public AccountController(UserManager userManager)
        {
            this.userManager = userManager;
        }

        [HttpGet, Authorize]
        public IActionResult Index()
        {
            var model = new LogInViewModel()
            {
                Name = HttpContext.User.Identity.Name
            };
            return View(model);
        }

        [HttpGet]
        public IActionResult LogIn()
        {
            var model = new LogInViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> LogIn(LogInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {

                var returnUrl = ((string)Request.Query["ReturnUrl"]) ?? "/";
                await userManager.SignInAsync(this.HttpContext, model);
                return Redirect(returnUrl);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("summary", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await userManager.SignOutAsync(this.HttpContext);
            return RedirectToAction("Index", "Home");
        }
    }
}
