using MarketAI.API.Enums;
using MarketWB.Web.Helpers;
using MarketWB.Web.Models;
using MarketWB.Web.Models.Home;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = UserHelper.GetUser(User);
                if(user != null)
                {
                    if (user.UserRole == UserRole.User)
                        return View("Views/Cabinet/Dashboard/Dashboard.cshtml");
                    else if (user.UserRole == UserRole.Admin)
                        return RedirectToAction("Visitors", "Stats");
                    else
                        return Login();
                }
          
            }
            return Login();
        }
        [Route("Login")]
        public IActionResult Login()
        {
            return View("Views/Home/Login.cshtml");
        }


        [Route("Registration")]
        public IActionResult Registration()
        {
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
