using MarketWB.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Subscriptions = MarketAI.API.Controllers.SubscriptionsModule;
using Posts = MarketAI.API.Controllers.PostsModule;
using MarketWB.Web.ViewModels.Cabinet;
using MarketWB.Web.ViewModels.Cabinet.Dashboard;

namespace MarketWB.Web.Controllers
{
    [Authorize(Roles = "User")]
    public class ReferralController : Controller
    {
        private readonly ILogger<ReferralController> _logger;
        private readonly Subscriptions _subscriptions;
        private readonly Posts _posts;
        public ReferralController(ILogger<ReferralController> logger, Subscriptions subscriptions,Posts posts)
        {
            _logger = logger;
            _subscriptions = subscriptions;
            _posts = posts;
        }
        
        [Route("Cabinet/Referrals")]
        public IActionResult Referrals()
        {
            return View("Views/Cabinet/Referrals/Dashboard.cshtml");
        }
        [Route("Cabinet/WithdrawHistory")]
        public async Task<IActionResult> WithdrawHistory()
        {
            var user = await UserHelper.GetUser(User);
            return View("Views/Cabinet/Referrals/WithdrawHistory.cshtml", user);
        }



    }
}
