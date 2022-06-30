using MarketAI.API.Enums;
using MarketAI.API.Models;
using MarketWB.Web.Helpers;
using MarketWB.Web.Models;
using MarketWB.Web.Models.Home;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MarketWB.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [Route("/")]
        public async Task<IActionResult> Index()
        {
            return View("Views/Home/Index.cshtml");
        }
        [Route("Login")]
        public IActionResult Login()
        {
            return View("Views/Home/Login.cshtml");
        }


        [Route("Registration")]
        public IActionResult Registration(string invite = null,string email = null)
        {
            ViewData["refcode"] = invite;
            ViewData["email"] = email;
            return View();
        }
        [Route("Restore")]
        public IActionResult Restore()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        private async Task<ClaimsPrincipal> MakeAuth(UserModel user)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // создаем один claim
            var claims = new List<Claim>
            {
                 new Claim(ClaimsIdentity.DefaultNameClaimType, user.Id.ToString()),
                 new Claim(ClaimsIdentity.DefaultRoleClaimType, user.UserRole.ToString())
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            var principal = new ClaimsPrincipal(id);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return principal;
        }





        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> Error()
        {
            //return await Index();
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
