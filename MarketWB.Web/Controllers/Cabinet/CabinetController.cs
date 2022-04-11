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
using MarketWB.Web.ViewModels.Cabinet;

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
            var posts  = _posts.GetPosts().Where(o => !o.PublishAfter
                                           || (o.PublishAfter && o.PublishAfterDate < DateTime.Now));
            var vm = new NewsViewModel
            {
                Posts = posts,
                Tags = _posts.GetAllTags().ToList()
            };
            vm.Tags.Insert(0, "Все новости");
            return View("Views/Cabinet/News.cshtml", vm);
        }
        [Route("Cabinet/News/{tag}")]
        public IActionResult News(string tag)
        {
            var vm = new NewsViewModel
            {
                Posts = _posts.GetPosts(),
                Tags = _posts.GetAllTags().ToList()
            };
            vm.Tags.Insert(0, "Все новости");

            if (tag != "Все новости")
                vm.Posts = vm.Posts.Where(o => o.Tags.Split(',').Any(o => o == tag)).ToList();

            return View("Views/Cabinet/News.cshtml", vm);
        }


        [Route("Cabinet/Profile")]
        public async Task<IActionResult> Profile()
        {
            var user =await UserHelper.GetUser(User);
            return View("Views/Cabinet/Profile.cshtml", user);
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

        [Route("Cabinet/NeedToPay")]
        public IActionResult NeedToPay()
        {
            var subscriptions = _subscriptions.GetSubscriptions();
            return View("Views/Cabinet/NeedToPay.cshtml", subscriptions);
        }
    }
}
