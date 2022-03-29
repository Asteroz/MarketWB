using MarketWB.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subscriptions = MarketAI.API.Controllers.SubscriptionsController;
using Posts = MarketAI.API.Controllers.PostsController;

namespace MarketWB.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class CabinetController : Controller
    {
        private readonly ILogger<CabinetController> _logger;
        private readonly Subscriptions _subscriptions;
        private readonly Posts _posts;
        public CabinetController(ILogger<CabinetController> logger, Subscriptions subscriptions,Posts posts)
        {
            _logger = logger;
            _subscriptions = subscriptions;
            _posts = posts;
        }
        [Route("Cabinet/News")]
        public IActionResult News()
        {
            var posts = _posts.GetPosts();
            return View("Views/Cabinet/News.cshtml", posts);
        }
        [Route("Cabinet/Profile")]
        public IActionResult Profile()
        {
            return View("Views/Cabinet/Profile.cshtml");
        }
        [Route("Cabinet/About")]
        public IActionResult About()
        {
            return View("Views/Cabinet/About.cshtml");
        }
        [Route("Cabinet/Support")]
        public IActionResult Support()
        {
            return View("Views/Cabinet/Support.cshtml");
        }
        [Route("Cabinet/Analysis")]
        public IActionResult Analysis()
        {
            return View("Views/Cabinet/Analysis.cshtml");
        }
        [Route("Cabinet/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View("Views/Cabinet/ChangePassword.cshtml");
        }
        [Route("Cabinet/Subscriptions")]
        public IActionResult Subscriptions()
        {
            var subscriptions = _subscriptions.GetSubscriptions();
            return View("Views/Cabinet/Subscriptions.cshtml", subscriptions);
        }
    }
}
